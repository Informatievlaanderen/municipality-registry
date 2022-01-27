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
            var schemaRegistryUrl = configuration["Kafka:SchemaRegistryUrl"];
            _kafkaOptions = new KafkaOptions(bootstrapServers, schemaRegistryUrl);
            _topic = $"{configuration[_municipalityTopicKey]}" ?? throw new ArgumentException($"Configuration has no value for {_municipalityTopicKey}");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCorrected>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasDrawn>>(async (context, message, ct) =>
            {
                await Produce(message.Message.MunicipalityId, message.Message.ToContract(), cancellationToken);
            });
        }

        private async Task Produce<T>(Guid municipalityId, T message, CancellationToken cancellationToken = default)
            where T : class, IQueueMessage
        {
            var result = await KafkaProducer.Produce(_kafkaOptions, _topic, municipalityId.ToString("D"), message, EventsJsonSerializerSettingsProvider.CreateSerializerSettings(), cancellationToken);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Error + Environment.NewLine + result.ErrorReason); //TODO: create custom exception
        }
    }

    public static class KafkaProducer
    {
        public static async Task<Result<T>> Produce<T>(
            KafkaOptions options,
            string topic,
            string key,
            T message,
            JsonSerializerSettings jsonSerializerSettings,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var config = new ProducerConfig
            {
                BootstrapServers = options.BootstrapServers,
                ClientId = Dns.GetHostName()
            };

            try
            {
                var jsonSerializer = JsonSerializer.CreateDefault(jsonSerializerSettings);
                var kafkaJsonMessage = KafkaJsonMessage.Create<T>(message, jsonSerializer);

                using var producer = new ProducerBuilder<string, string>(config)
                    .SetValueSerializer(Serializers.Utf8)
                    .Build();

                _ = await producer.ProduceAsync(topic, new Message<string, string> { Value = jsonSerializer.Serialize(kafkaJsonMessage), Key = key }, cancellationToken);
                return Result<T>.Success(message);
            }
            catch (ProduceException<Null, T> ex)
            {
                return Result<T>.Failure(ex.Error.Code.ToString(), ex.Error.Reason);
            }
            catch (OperationCanceledException)
            {
                return Result<T>.Success(default);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message, string.Empty);
            }
        }
    }
    public class KafkaJsonMessage
    {
        public string Type { get; set; }
        public string Data { get; set; }

        public KafkaJsonMessage(string type, string data)
        {
            Type = type;
            Data = data;
        }

        public static KafkaJsonMessage Create<T>([DisallowNull] T message, JsonSerializer serializer)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            return new KafkaJsonMessage(message.GetType().FullName, serializer.Serialize(message));
        }
    }

    public static class JsonSerializerExtensions
    {
        public static string Serialize(
            this JsonSerializer jsonSerializer,
            object value)
        {
            var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.Formatting = jsonSerializer.Formatting;
                jsonSerializer.Serialize(jsonTextWriter, value, value.GetType());
            }

            return stringWriter.ToString();
        }
    }
}
