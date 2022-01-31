namespace MunicipalityRegistry.Producer
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Confluent.Kafka;
    using Extensions;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Domain = Municipality.Events;

    [ConnectedProjectionName("Kafka producer")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly string _topic;
        private string _municipalityTopicKey = "MunicipalityTopic";

        public ProducerProjections(IConfiguration configuration)
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"];
            _kafkaOptions = new KafkaOptions(bootstrapServers, EventsJsonSerializerSettingsProvider.CreateSerializerSettings());
            _topic = $"{configuration[_municipalityTopicKey]}" ?? throw new ArgumentException($"Configuration has no value for {_municipalityTopicKey}");

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasDrawn>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            });
        }

        private async Task Produce<T>(Guid municipalityId, T message, CancellationToken cancellationToken = default)
            where T : class, IQueueMessage
        {
            var result = await KafkaProducer.Produce(_kafkaOptions, _topic, municipalityId.ToString("D"), message, cancellationToken);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Error + Environment.NewLine + result.ErrorReason); //TODO: create custom exception
        }
    }
}
