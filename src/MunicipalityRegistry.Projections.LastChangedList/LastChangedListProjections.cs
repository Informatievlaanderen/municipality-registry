namespace MunicipalityRegistry.Projections.LastChangedList
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;

    [ConnectedProjectionName("LastChangedList")]
    [ConnectedProjectionDescription("Markeert de gemeentes waarvan de cached data moet geupdate worden.")]
    public class LastChangedListProjections : LastChangedListConnectedProjection
    {
        protected override string CacheKeyFormat => "legacy/municipality:{{0}}.{1}";
        protected override string UriFormat => "/v1/gemeenten/{{0}}";

        private static readonly AcceptType[] SupportedAcceptTypes = { AcceptType.Json, AcceptType.Xml };

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

        private static void DoNothing() { }
    }
}
