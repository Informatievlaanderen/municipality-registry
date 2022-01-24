namespace MunicipalityRegistry.Producer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.BasisRegisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.Extensions.Configuration;
    using Municipality.Events;

    [ConnectedProjectionName("Kafka producer")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        public ProducerProjections(IConfiguration configuration)
        {
            string kafkaBootstrapServers = configuration["Kafka:BootstrapServers"];
            string kafkaSchemaRegistryUrl = configuration["Kafka:SchemaRegistryUrl"];
            
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });

            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) =>
            {
                await Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, message.Message, cancellationToken);
            });
        }

        private async Task Produce<T>(string kafkaBootstrapServers, string kafkaSchemaRegistryUrl, T message, CancellationToken cancellationToken = default)
            where T : class
        {
            const string topic = "municipality";
            _ = await KafkaProducer.Produce(kafkaBootstrapServers, kafkaSchemaRegistryUrl, topic, message, cancellationToken);
        }
    }
}
