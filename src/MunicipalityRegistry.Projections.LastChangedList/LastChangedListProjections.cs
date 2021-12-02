namespace MunicipalityRegistry.Projections.LastChangedList
{
    using System;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;

    [ConnectedProjectionName("Cache markering gemeenten")]
    [ConnectedProjectionDescription("Projectie die markeert voor hoeveel gemeenten de gecachte data nog geüpdated moeten worden.")]
    public class LastChangedListProjections : LastChangedListConnectedProjection
    {
        private static readonly AcceptType[] SupportedAcceptTypes = { AcceptType.Json, AcceptType.Xml, AcceptType.JsonLd };

        public LastChangedListProjections()
            : base(SupportedAcceptTypes)
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

            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
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

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
        }

        protected override string BuildCacheKey(AcceptType acceptType, string identifier)
        {
            var shortenedAcceptType = acceptType.ToString().ToLowerInvariant();
            return acceptType switch
            {
                AcceptType.Json => string.Format("legacy/municipality:{{0}}.{1}", identifier, shortenedAcceptType),
                AcceptType.Xml => string.Format("legacy/municipality:{{0}}.{1}", identifier, shortenedAcceptType),
                AcceptType.JsonLd => string.Format("oslo/municipality:{{0}}.{1}", identifier, shortenedAcceptType),
                _ => throw new NotImplementedException($"Cannot build CacheKey for type {typeof(AcceptType)}")
            };
        }

        protected override string BuildUri(AcceptType acceptType, string identifier)
        {
            return acceptType switch
            {
                AcceptType.Json => string.Format("/v1/gemeenten/{{0}}", identifier),
                AcceptType.Xml => string.Format("/v1/gemeenten/{{0}}", identifier),
                AcceptType.JsonLd => string.Format("/v2/gemeenten/{{0}}", identifier),
                _ => throw new NotImplementedException($"Cannot build Uri for type {typeof(AcceptType)}")
            };
        }

        private static void DoNothing() { }
    }
}
