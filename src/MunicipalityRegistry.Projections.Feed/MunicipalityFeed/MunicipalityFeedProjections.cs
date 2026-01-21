namespace MunicipalityRegistry.Projections.Feed.MunicipalityFeed
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.Model;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using CloudNative.CloudEvents;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using Contract;
    using Microsoft.EntityFrameworkCore;
    using Municipality.Events;
    using Newtonsoft.Json;

    public sealed class MunicipalityFeedConfig
    {
        public required string Namespace { get; set; }
        public required string FeedUrl { get; set; }
        public required string DataSchemaUrl { get; set; }
        public required string DataSchemaUrlTransform { get; set; }
    }

    [ConnectedProjectionName("Feed endpoint gemeenten")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor de gemeenten cloudevent feed voorziet.")]
    public class MunicipalityFeedProjections : ConnectedProjection<FeedContext>
    {
        public const int MaxPageSize = 100;

        private static readonly CloudEventAttribute EventTypeAttribute =
            CloudEventAttribute.CreateExtension(BaseRegistriesCloudEventAttribute.BaseRegistriesEventType, CloudEventAttributeType.String);
        private static readonly CloudEventAttribute CausationIdAttribute =
            CloudEventAttribute.CreateExtension(BaseRegistriesCloudEventAttribute.BaseRegistriesCausationId, CloudEventAttributeType.String);

        public static readonly IReadOnlyList<CloudEventAttribute> ExtensionAttributes =
            [EventTypeAttribute, CausationIdAttribute];

        private readonly MunicipalityFeedConfig _feedConfig;
        private readonly LastChangedListContext _lastChangedListContext;
        private readonly JsonEventFormatter _jsonEventFormatter;

        private readonly Uri _feedSourceUri;
        private readonly Uri _dataSchemaUri;
        private readonly Uri _dataSchemaUriTransform;

        public MunicipalityFeedProjections(
            MunicipalityFeedConfig feedConfig,
            LastChangedListContext  lastChangedListContext,
            JsonSerializerSettings jsonSerializerSettings)
        {
            _feedConfig = feedConfig;
            _lastChangedListContext = lastChangedListContext;
            _jsonEventFormatter = new JsonEventFormatter(JsonSerializer.Create(jsonSerializerSettings));

            _feedSourceUri = new Uri(feedConfig.FeedUrl);
            _dataSchemaUri = new Uri(feedConfig.DataSchemaUrl);
            _dataSchemaUriTransform = new Uri(feedConfig.DataSchemaUrlTransform);

            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                var document = new MunicipalityDocument(message.Message.MunicipalityId, message.Message.NisCode, message.Message.Provenance.Timestamp);
                await context.MunicipalityDocuments.AddAsync(document, ct);

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.NisCode, null, document.NisCode),
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.StatusName, null,
                        GemeenteStatus.Voorgesteld)
                ], MunicipalityEventTypes.CreateV1);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldNisCode = document.Document.NisCode;
                document.NisCode = message.Message.NisCode;
                document.Document.NisCode = message.Message.NisCode;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.NisCode, oldNisCode, document.NisCode)
                ]);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldNisCode = document.Document.NisCode;
                document.NisCode = message.Message.NisCode;
                document.Document.NisCode = message.Message.NisCode;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.NisCode, oldNisCode, document.NisCode)
                ]);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldNames = document.Document.Names.ToList();
                document.Document.Names.Add(new GeografischeNaam(message.Message.Name, MapLanguage(message.Message.Language)));
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context,
                [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.MunicipalityNames, oldNames, document.Document.Names)
                ]);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldNames = document.Document.Names
                    .Select(x => new GeografischeNaam(x.Spelling, x.Taal))
                    .ToList();

                var name = document.Document.Names.Single(x => x.Taal == MapLanguage(message.Message.Language));
                name.Spelling = message.Message.Name;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context,
                [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.MunicipalityNames, oldNames, document.Document.Names)
                ]);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldNames = document.Document.Names.ToList();
                var name = document.Document.Names.Single(x => x.Taal == MapLanguage(message.Message.Language));
                document.Document.Names.Remove(name);
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.MunicipalityNames, oldNames, document.Document.Names)
                ]);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldNames = document.Document.Names.ToList();
                var name = document.Document.Names.Single(x => x.Taal == MapLanguage(message.Message.Language));
                document.Document.Names.Remove(name);
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.MunicipalityNames, oldNames, document.Document.Names)
                ]);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldOfficialLanguages = document.Document.OfficialLanguages.ToList();
                document.Document.OfficialLanguages.Add(MapLanguage(message.Message.Language));
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.OfficialLanguages, oldOfficialLanguages, document.Document.OfficialLanguages)
                ]);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldOfficialLanguages = document.Document.OfficialLanguages.ToList();
                document.Document.OfficialLanguages.Remove(MapLanguage(message.Message.Language));
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.OfficialLanguages, oldOfficialLanguages, document.Document.OfficialLanguages)
                ]);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldFacilitiesLanguages = document.Document.FacilitiesLanguages.ToList();
                document.Document.FacilitiesLanguages.Add(MapLanguage(message.Message.Language));
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.FacilitiesLanguages, oldFacilitiesLanguages, document.Document.OfficialLanguages)
                ]);
            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldFacilitiesLanguages = document.Document.FacilitiesLanguages.ToList();
                document.Document.FacilitiesLanguages.Remove(MapLanguage(message.Message.Language));
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.FacilitiesLanguages, oldFacilitiesLanguages, document.Document.OfficialLanguages)
                ]);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                document.Document.Status = GemeenteStatus.InGebruik;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.StatusName, GemeenteStatus.Voorgesteld, document.Document.Status)
                ]);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldStatus = document.Document.Status;
                document.Document.Status = GemeenteStatus.InGebruik;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.StatusName, oldStatus, document.Document.Status)
                ]);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldStatus = document.Document.Status;
                document.Document.Status = GemeenteStatus.Gehistoreerd;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.StatusName, oldStatus, document.Document.Status)
                ]);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                var oldStatus = document.Document.Status;
                document.Document.Status = GemeenteStatus.Gehistoreerd;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.StatusName, oldStatus, document.Document.Status)
                ]);
            });

            When<Envelope<MunicipalityWasMerged>>(async (context, message, ct) =>
            {
                var nisCodes = new List<string>(message.Message.NisCodesToMergeWith)
                {
                    message.Message.NewNisCode,
                    message.Message.NisCode
                };

                var page = await context.CalculatePage();
                var municipalityFeedItem = new MunicipalityFeedItem(
                    position: message.Position,
                    page: page,
                    municipalityId: message.Message.MunicipalityId,
                    nisCode: message.Message.NisCode)
                {
                    Application = message.Message.Provenance.Application,
                    Modification = message.Message.Provenance.Modification,
                    Operator = message.Message.Provenance.Operator,
                    Organisation = message.Message.Provenance.Organisation,
                    Reason = message.Message.Provenance.Reason,
                };
                await context.MunicipalityFeed.AddAsync(municipalityFeedItem, ct);

                var cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0, ExtensionAttributes)
                {
                    Id = municipalityFeedItem.Id.ToString(CultureInfo.InvariantCulture),
                    Time = message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset(),
                    Type = MunicipalityEventTypes.TransformV1,
                    Source = _feedSourceUri,
                    DataContentType = MediaTypeNames.Application.Json,
                    Data = new BaseRegistriesCloudTransformEvent
                    {
                       From = message.Message.NisCode,
                       To = message.Message.NewNisCode,
                       NisCodes = nisCodes
                    },
                    DataSchema = _dataSchemaUriTransform,
                    [BaseRegistriesCloudEventAttribute.BaseRegistriesEventType] = message.EventName,
                    [BaseRegistriesCloudEventAttribute.BaseRegistriesCausationId] = message.Metadata["CommandId"].ToString()
                };

                cloudEvent.Validate();
                municipalityFeedItem.CloudEventAsString = SerializeCloudEvent(cloudEvent);
                await CheckToUpdateCache(page, context);
            });

            When<Envelope<MunicipalityWasRemoved>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context, [], MunicipalityEventTypes.DeleteV1);
            });

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
        }

        private async Task AddCloudEvent<T>(
            Envelope<T> message,
            MunicipalityDocument document,
            FeedContext context,
            List<BaseRegistriesCloudEventAttribute> attributes,
            string eventType = MunicipalityEventTypes.UpdateV1)
            where T : IHasProvenance, IMessage
        {
            context.Entry(document).Property(x => x.Document).IsModified = true;

            var page = await context.CalculatePage();
            var municipalityFeedItem = new MunicipalityFeedItem(
                position: message.Position,
                page: page,
                municipalityId: document.MunicipalityId,
                nisCode: document.NisCode)
            {
                Application = message.Message.Provenance.Application,
                Modification = message.Message.Provenance.Modification,
                Operator = message.Message.Provenance.Operator,
                Organisation = message.Message.Provenance.Organisation,
                Reason = message.Message.Provenance.Reason
            };
            await context.MunicipalityFeed.AddAsync(municipalityFeedItem);

            var cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0, ExtensionAttributes)
            {
                Id = municipalityFeedItem.Id.ToString(CultureInfo.InvariantCulture),
                Time = message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset(),
                Type = eventType,
                Source = _feedSourceUri,
                DataContentType = MediaTypeNames.Application.Json,
                Data = new BaseRegistriesCloudEvent
                {
                    Id = $"{_feedConfig.Namespace}/{document.NisCode}",
                    ObjectId = document.NisCode,
                    Namespace = _feedConfig.Namespace,
                    VersionId = new Rfc3339SerializableDateTimeOffset(document.LastChangedOnAsDateTimeOffset).ToString(),
                    NisCodes = [document.NisCode],
                    Attributes = attributes
                },
                DataSchema = _dataSchemaUri,
                [BaseRegistriesCloudEventAttribute.BaseRegistriesEventType] = message.EventName,
                [BaseRegistriesCloudEventAttribute.BaseRegistriesCausationId] = message.Metadata["CommandId"].ToString()
            };

            cloudEvent.Validate();
            municipalityFeedItem.CloudEventAsString = SerializeCloudEvent(cloudEvent);
            await CheckToUpdateCache(page, context);
        }

        public static Taal MapLanguage(Language language)
        {
            switch (language)
            {
                default:
                case Language.Dutch:
                    return Taal.NL;

                case Language.French:
                    return Taal.FR;

                case Language.German:
                    return Taal.DE;

                case Language.English:
                    return Taal.EN;
            }
        }

        private string SerializeCloudEvent(CloudEvent cloudEvent)
        {
            var bytes = _jsonEventFormatter.EncodeStructuredModeMessage(cloudEvent, out _);
            return Encoding.UTF8.GetString(bytes.Span);
        }

        private async Task CheckToUpdateCache(int page, FeedContext feedContext)
        {
            var pageItemsCount = await feedContext.MunicipalityFeed.CountAsync(x => x.Page == page);
            if(pageItemsCount < (MaxPageSize-1))
                return;

            await feedContext.SaveChangesAsync();
            await _lastChangedListContext.LastChangedList.AddAsync(new LastChangedRecord
            {
                AcceptType = "application/cloudevents-batch+json",
                CacheKey = $"feed/municipality:{page}",
                Id = $"{page}.v1.feed",
                Position = page,
                Uri = $"/v2/gemeenten/wijzigingen?page={page}"
            });
            await _lastChangedListContext.SaveChangesAsync();
        }

        private static async Task DoNothing()
        {
            await Task.Yield();
        }
    }
}
