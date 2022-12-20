namespace MunicipalityRegistry.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Producer;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Extensions;
    using Domain = Municipality.Events;

    [ConnectedProjectionName("Kafka producer")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        public const string MunicipalityTopicKey = "MunicipalityTopic";

        private readonly IProducer _producer;
        public ProducerProjections(IProducer producer)
        {
            _producer = producer;

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasRegistered>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNisCodeWasDefined>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNisCodeWasCorrected>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasNamed>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNameWasCorrected>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNameWasCorrectedToCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityOfficialLanguageWasAdded>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityOfficialLanguageWasRemoved>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityFacilityLanguageWasAdded>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityFacilityLanguageWasRemoved>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityBecameCurrent>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasCorrectedToCurrent>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasRetired>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasCorrectedToRetired>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCorrected>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCorrectedToCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasDrawn>>(async (_, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), message.Position, ct);
            });
        }

        private async Task Produce<T>(
            Guid municipalityId,
            T message,
            long storePosition,
            CancellationToken cancellationToken = default)
            where T : class, IQueueMessage
        {
            var result = await _producer.ProduceJsonMessage(
                new MessageKey(municipalityId.ToString("D")),
                message,
                new List<MessageHeader> { new MessageHeader(MessageHeader.IdempotenceKey, storePosition.ToString()) },
                cancellationToken);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Error + Environment.NewLine + result.ErrorReason); //TODO: create custom exception
            }
        }
    }
}
