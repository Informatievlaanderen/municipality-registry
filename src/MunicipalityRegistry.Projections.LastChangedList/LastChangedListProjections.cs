namespace MunicipalityRegistry.Projections.LastChangedList
{
    using System;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;

    [ConnectedProjectionName(ProjectionName)]
    [ConnectedProjectionDescription("Projectie die markeert voor hoeveel gemeenten de gecachte data nog geüpdated moeten worden.")]
    public class LastChangedListProjections : LastChangedListConnectedProjection
    {
        public const string ProjectionName = "Cache markering gemeenten";
        private static readonly AcceptType[] SupportedAcceptTypes = { AcceptType.JsonLd };

        public LastChangedListProjections(ICacheValidator cacheValidator)
            : base(SupportedAcceptTypes, cacheValidator)
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                var attachedRecords = await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);

                foreach (var record in attachedRecords)
                {
                    record.CacheKey = string.Format(record.CacheKey, message.Message.NisCode);
                    record.Uri = string.Format(record.Uri, message.Message.NisCode);
                }
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                var attachedRecords = await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);

                foreach (var record in attachedRecords)
                {
                    record.CacheKey = string.Format(record.CacheKey, message.Message.NisCode);
                    record.Uri = string.Format(record.Uri, message.Message.NisCode);
                }
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                var attachedRecords = await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);

                foreach (var record in attachedRecords)
                {
                    record.CacheKey = string.Format(record.CacheKey, message.Message.NisCode);
                    record.Uri = string.Format(record.Uri, message.Message.NisCode);
                }
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.MunicipalityId.ToString(), message.Position, context, ct);
            });

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
        }

        protected override string BuildCacheKey(AcceptType acceptType, string identifier)
        {
            var shortenedAcceptType = acceptType.ToString().ToLowerInvariant();
            return acceptType switch
            {
                AcceptType.JsonLd => $"oslo/municipality:{{0}}.{shortenedAcceptType}",
                _ => throw new NotImplementedException($"Cannot build CacheKey for type {typeof(AcceptType)}")
            };
        }

        protected override string BuildUri(AcceptType acceptType, string identifier)
        {
            return acceptType switch
            {
                AcceptType.JsonLd => $"/v2/gemeenten/{{0}}",
                _ => throw new NotImplementedException($"Cannot build Uri for type {typeof(AcceptType)}")
            };
        }

        private static async Task DoNothing()
        {
            await Task.Yield();
        }
    }
}
