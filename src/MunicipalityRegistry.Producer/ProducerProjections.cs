namespace MunicipalityRegistry.Producer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Muni = Municipality.Events;

    [ConnectedProjectionName("Kafka producer")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly string _topic;

        public ProducerProjections(IConfiguration configuration, IHostEnvironment env)
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"];
            var schemaRegistryUrl = configuration["Kafka:SchemaRegistryUrl"];
            _kafkaOptions = new KafkaOptions(bootstrapServers, schemaRegistryUrl);
            _topic = $"{env.EnvironmentName}/{configuration["MunicipalityTopic"] ?? "municipality"}";
            
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityGeometryWasCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityGeometryWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Muni.MunicipalityWasDrawn>>(async (context, message, ct) =>
            {
                await Produce(message.Message.ToContract(), cancellationToken);
            });
        }

        private async Task Produce<T>(T message, CancellationToken cancellationToken = default)
            where T : class, IQueueMessage
        {
            _ = await KafkaProducer.Produce(_kafkaOptions, _topic, message, cancellationToken);
        }
    }
}
