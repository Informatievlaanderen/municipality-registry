namespace MunicipalityRegistry.Projections.StreamPublisher.MessageDetail
{
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Extensions;
    using Municipality.Events;

    [ConnectedProjectionName("API endpoint detail gemeenten events")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor het gemeenten streampublisher voorziet.")]
    public class MessageDetailProjections : ConnectedProjection<StreamPublisherContext>
    {
        public MessageDetailProjections()
        {
            var publisher = new EventPublisher(default);

            When<Envelope<MunicipalityBecameCurrent>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityFacilityLanguageWasAdded>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityGeometryWasCleared>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityGeometryWasCorrected>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityNameWasCleared>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityNameWasCorrected>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityNameWasCorrectedToCleared>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityNisCodeWasCorrected>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityNisCodeWasDefined>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityOfficialLanguageWasAdded>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityWasCorrectedToCurrent>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityWasCorrectedToRetired>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityWasDrawn>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityWasNamed>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityWasRegistered>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityWasRetired>>((context, message, ct) => Publish(message, publisher));

            // CRAB events

            When<Envelope<MunicipalityNameWasImportedFromCrab>>((context, message, ct) => Publish(message, publisher));
            When<Envelope<MunicipalityWasImportedFromCrab>>((context, message, ct) => Publish(message, publisher));
        }

        private static Task Publish<TMessage>(Envelope<TMessage> message, EventPublisher publisher)
            where TMessage : IMunicipalityMessage
        {
            var envelope = message.Message.ToContract();
            publisher.Publish(envelope);
            return Task.CompletedTask;
        }
    }
}
