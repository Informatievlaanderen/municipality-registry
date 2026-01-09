namespace MunicipalityRegistry.Projections.Feed.MunicipalityFeed
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using CloudNative.CloudEvents;
    using Contract;
    using Municipality.Events;
    using Newtonsoft.Json;

    public sealed class MunicipalityFeedConfig
    {
        public required string Namespace { get; set; }
        public required string FeedUrl { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = null!;
    }

    [ConnectedProjectionName("Feed endpoint gemeenten")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor de gemeenten cloudevent feed voorziet.")]
    public class MunicipalityFeedProjections : ConnectedProjection<FeedContext>
    {
        private readonly MunicipalityFeedConfig _feedConfig;

        public MunicipalityFeedProjections(MunicipalityFeedConfig feedConfig)
        {
            _feedConfig = feedConfig;

            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                var document = new MunicipalityDocument(message.Message.MunicipalityId, message.Message.NisCode, message.Message.Provenance.Timestamp);
                await context.MunicipalityDocuments.AddAsync(document, ct);

                var cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0, new List<CloudEventAttribute>
                {
                    CloudEventAttribute.CreateExtension(BaseRegistriesCloudEventAttribute.BaseRegistriesEventType, CloudEventAttributeType.String),
                    CloudEventAttribute.CreateExtension(BaseRegistriesCloudEventAttribute.BaseRegistriesCausationId, CloudEventAttributeType.String),
                })
                {
                    Id = message.Position.ToString(),
                    Time = message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset(),
                    Type = MunicipalityEventTypes.CreateV1,
                    Source = new Uri(feedConfig.FeedUrl),
                    DataContentType = MediaTypeNames.Application.Json,
                    Data = new BaseRegistriesCloudEvent
                    {
                        Id = $"{feedConfig.Namespace}/{message.Message.MunicipalityId.ToString()}",
                        ObjectId = document.NisCode,
                        Namespace = feedConfig.Namespace,
                        VersionId = new Rfc3339SerializableDateTimeOffset(document.LastChangedOnAsDateTimeOffset).ToString(),
                        NisCodes = [message.Message.NisCode],
                        Attributes = [
                            new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.NisCode, null, message.Message.NisCode),
                            new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.StatusName, null, GemeenteStatus.Voorgesteld)
                        ],
                    },
                    DataSchema = new Uri("https://api.basisregisters.vlaanderen.be/schemas/municipality.json"),
                    [BaseRegistriesCloudEventAttribute.BaseRegistriesEventType] = message.EventName,
                    [BaseRegistriesCloudEventAttribute.BaseRegistriesCausationId] = message.Metadata["CommandId"].ToString()
                };

                cloudEvent.Validate();

                await context.MunicipalityFeed.AddAsync(new MunicipalityFeedItem(
                    position: message.Position,
                    page: await context.CalculatePage(),
                    municipalityId: message.Message.MunicipalityId,
                    nisCode: message.Message.NisCode)
                {
                    Application = message.Message.Provenance.Application,
                    Modification = message.Message.Provenance.Modification,
                    Operator = message.Message.Provenance.Operator,
                    Organisation = message.Message.Provenance.Organisation,
                    Reason = message.Message.Provenance.Reason,
                    CloudEvent = cloudEvent
                });
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                var document = await context.MunicipalityDocuments.FindAsync(message.Message.MunicipalityId, ct);
                if (document == null)
                    throw new InvalidOperationException($"Could not find document for municipality {message.Message.MunicipalityId}");

                document.NisCode = message.Message.NisCode;
                document.Document.NisCode = message.Message.NisCode;
                document.LastChangedOn = message.Message.Provenance.Timestamp;

                await AddCloudEvent(message, document, context);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityWasMerged>>(async (context, message, ct) =>
            {

            });

            When<Envelope<MunicipalityWasRemoved>>(async (context, message, ct) =>
            {

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
            FeedContext context)
            where T : IHasProvenance, IMessage
        {
            var cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0, new List<CloudEventAttribute>
            {
                CloudEventAttribute.CreateExtension(BaseRegistriesCloudEventAttribute.BaseRegistriesEventType, CloudEventAttributeType.String),
                CloudEventAttribute.CreateExtension(BaseRegistriesCloudEventAttribute.BaseRegistriesCausationId, CloudEventAttributeType.String),
            })
            {
                Id = message.Position.ToString(),
                Time = message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset(),
                Type = MunicipalityEventTypes.UpdateV1,
                Source = new Uri(_feedConfig.FeedUrl),
                DataContentType = MediaTypeNames.Application.Json,
                Data = new BaseRegistriesCloudEvent
                {
                    Id = $"{_feedConfig.Namespace}/{document.NisCode}",
                    ObjectId = document.NisCode,
                    Namespace = _feedConfig.Namespace,
                    VersionId = new Rfc3339SerializableDateTimeOffset(document.LastChangedOnAsDateTimeOffset).ToString(),
                    NisCodes = [document.NisCode],
                    Attributes = []
                },
                DataSchema = new Uri("https://api.basisregisters.vlaanderen.be/schemas/municipality.json"),
                [BaseRegistriesCloudEventAttribute.BaseRegistriesEventType] = message.EventName,
                [BaseRegistriesCloudEventAttribute.BaseRegistriesCausationId] = message.Metadata["CommandId"].ToString()
            };

            cloudEvent.Validate();

            await context.MunicipalityFeed.AddAsync(new MunicipalityFeedItem(
                position: message.Position,
                page: await context.CalculatePage(),
                municipalityId: document.MunicipalityId,
                nisCode: document.NisCode)
            {
                Application = message.Message.Provenance.Application,
                Modification = message.Message.Provenance.Modification,
                Operator = message.Message.Provenance.Operator,
                Organisation = message.Message.Provenance.Organisation,
                Reason = message.Message.Provenance.Reason,
                CloudEvent = cloudEvent
            });
        }

        // private static void UpdateNameByLanguage(MunicipalitySyndicationItem municipalitySyndicationItem, Language? language, string name)
        // {
        //     switch (language)
        //     {
        //         case Language.Dutch:
        //             municipalitySyndicationItem.NameDutch = name;
        //             break;
        //
        //         case Language.French:
        //             municipalitySyndicationItem.NameFrench = name;
        //             break;
        //
        //         case Language.German:
        //             municipalitySyndicationItem.NameGerman = name;
        //             break;
        //
        //         case Language.English:
        //             municipalitySyndicationItem.NameEnglish = name;
        //             break;
        //     }
        // }
        //
        // private static void UpdateDefaultName(MunicipalitySyndicationItem municipalitySyndicationItem)
        // {
        //     switch (municipalitySyndicationItem.OfficialLanguages.FirstOrDefault())
        //     {
        //         default:
        //             municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameDutch;
        //             break;
        //
        //         case Language.French:
        //             municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameFrench;
        //             break;
        //
        //         case Language.German:
        //             municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameGerman;
        //             break;
        //
        //         case Language.English:
        //             municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameEnglish;
        //             break;
        //     }
        // }

        private static async Task DoNothing()
        {
            await Task.Yield();
        }
    }
}
