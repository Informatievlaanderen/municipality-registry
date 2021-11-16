namespace MunicipalityRegistry.Projections.QueuePublisher.MessageDetail
{
    using System;
    using Contracts = Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Municipality.Events;
    using Newtonsoft.Json;
    using NodaTime;

    [ConnectedProjectionName("API endpoint detail gemeenten events")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor het gemeenten queuepublisher voorziet.")]
    public class MessageDetailProjections : ConnectedProjection<QueuePublisherContext>
    {
        public MessageDetailProjections()
        {
            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                // var queueEvent = new Contracts.MunicipalityRegistry.MunicipalityWasRegistered(
                //     message.Message.MunicipalityId.ToString("D"),
                //     message.Message.NisCode,
                //     provenance: null);
                //
                // var envelope = new Contracts.Envelope<Contracts.IQueueMessage>()
                // {
                //     Id = Guid.NewGuid().ToString("D"),
                //     EventName = nameof(MunicipalityWasRegistered),
                //     Payload = JsonConvert.SerializeObject(queueEvent),
                //     Timestamp = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture)
                // };
                // var messageDetail = new MessageDetail()
                // {
                //     Id = Guid.Parse(envelope.Id),
                //     MessageType = "topic",
                //     QueueName = "events",
                //     Payload = envelope.Payload, //Convert internal event to IQueueEvent
                //     EventName = envelope.EventName,
                //     Timestamp = Instant.FromDateTimeUtc(DateTime.Parse(envelope.Timestamp))
                // };
                //
                // //Add to MessageDetail
                // await context.MessageDetail.AddAsync(messageDetail, ct);
                //
                // //Publish to MessageQueue (DI)
                // new EventPublisher(null).Publish(envelope);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
            });

        }
    }
}
