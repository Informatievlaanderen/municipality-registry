namespace MunicipalityRegistry.Projections.Feed.MunicipalityFeed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.ChangeFeed;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Oslo;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Contract;
    using Microsoft.EntityFrameworkCore;
    using Municipality.Events;

    [ConnectedProjectionName("Feed endpoint gemeenten")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor de gemeenten cloudevent feed voorziet.")]
    public class MunicipalityFeedProjections : ConnectedProjection<FeedContext>
    {
        private readonly IChangeFeedService _changeFeedService;

        public MunicipalityFeedProjections(IChangeFeedService changeFeedService)
        {
            _changeFeedService = changeFeedService;

            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                var document = new MunicipalityDocument(message.Message.MunicipalityId, message.Message.NisCode, message.Message.Provenance.Timestamp);
                await context.MunicipalityDocuments.AddAsync(document, ct);

                await AddCloudEvent(message, document, context, [
                    new BaseRegistriesCloudEventAttribute(MunicipalityAttributeNames.StatusName, null,
                        GemeenteStatus.Voorgesteld)
                ], MunicipalityEventTypes.CreateV1);
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

                var transformData = new MunicipalityCloudTransformEvent
                {
                    From = OsloNamespaces.Gemeente.ToPuri(message.Message.NisCode),
                    To = OsloNamespaces.Gemeente.ToPuri(message.Message.NewNisCode),
                    NisCodes = nisCodes
                };

                var cloudEvent = _changeFeedService.CreateCloudEvent(
                    municipalityFeedItem.Id,
                    message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset(),
                    MunicipalityEventTypes.TransformV1,
                    transformData,
                    _changeFeedService.DataSchemaUriTransform,
                    message.EventName,
                    message.Metadata["CommandId"].ToString()!);

                municipalityFeedItem.CloudEventAsString = _changeFeedService.SerializeCloudEvent(cloudEvent);
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
            // Minimal prevention of wrong event type.
            if (eventType == MunicipalityEventTypes.DeleteV1 && attributes.Count > 0)
            {
                throw new ArgumentException("Probably wrong event type!");
            }

            // Why is this?
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

            // Is the municipalityFeedItem.Id already known at this moment in time?
            // I'd think only upon SaveChanges as it is database generated?
            var cloudEvent = _changeFeedService.CreateCloudEventWithData(
                municipalityFeedItem.Id,
                message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset(),
                eventType,
                document.NisCode,
                document.LastChangedOnAsDateTimeOffset,
                [document.NisCode],
                attributes,
                message.EventName,
                message.Metadata["CommandId"].ToString()!);

            municipalityFeedItem.CloudEventAsString = _changeFeedService.SerializeCloudEvent(cloudEvent);

            // This method does a hidden SaveChanges on the FeedContext. This is not so explicit?
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

        private async Task CheckToUpdateCache(int page, FeedContext context)
        {
            await _changeFeedService.CheckToUpdateCacheAsync(
                page,
                context,
                async p => await context.MunicipalityFeed.CountAsync(x => x.Page == p));
        }

        private static async Task DoNothing()
        {
            await Task.Yield();
        }
    }
}
