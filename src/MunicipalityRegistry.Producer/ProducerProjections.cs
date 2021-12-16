namespace MunicipalityRegistry.Producer
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;

    [ConnectedProjectionName("Kafka producer")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        public ProducerProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
               //Send to kafka
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
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

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
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

            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
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

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) =>
            {
            });

            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) =>
            {
            });

            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) =>
            {
            });

            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) =>
            {
            });
        }
    }
}
