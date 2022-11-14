namespace MunicipalityRegistry.Producer.Snapshot.Oslo
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Confluent.Kafka;
    using Microsoft.Extensions.Configuration;
    using Municipality.Events;
    using MunicipalityRegistry.Projections.Legacy;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;

    [ConnectedProjectionName("Kafka producer")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public sealed class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        private readonly KafkaProducerOptions _kafkaOptions;
        private readonly string _TopicKey = "MunicipalityTopic";

        public ProducerProjections(IConfiguration configuration, ISnapshotManager snapshotManager, LegacyContext legacyContext)
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"];
            var osloNamespace = configuration["OsloNamespace"];
            osloNamespace = osloNamespace.TrimEnd('/');

            var topic = $"{configuration[_TopicKey]}" ?? throw new ArgumentException($"Configuration has no value for {_TopicKey}");
            _kafkaOptions = new KafkaProducerOptions(
                bootstrapServers,
                configuration["Kafka:SaslUserName"],
                configuration["Kafka:SaslPassword"],
                topic,
                false,
                EventsJsonSerializerSettingsProvider.CreateSerializerSettings());

            When<Envelope<MunicipalityBecameCurrent>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            // No impact on Oslo snapshot

            //When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityGeometryWasCleared>>(async (_, message, ct) =>
            //{
            //    if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
            //    {
            //        return;
            //    }

            //    var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

            //    await FindAndProduce(async () =>
            //            await snapshotManager.FindMatchingSnapshot(
            //                GetNisCode(municipalityDetail),
            //                message.Message.Provenance.Timestamp,
            //                throwStaleWhenGone: false,
            //                ct),
            //            ct);
            //});

            //When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityGeometryWasCorrected>>(async (_, message, ct) =>
            //{
            //    if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
            //    {
            //        return;
            //    }

            //    var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

            //    await FindAndProduce(async () =>
            //            await snapshotManager.FindMatchingSnapshot(
            //                GetNisCode(municipalityDetail),
            //                message.Message.Provenance.Timestamp,
            //                throwStaleWhenGone: false,
            //                ct),
            //            ct);
            //});

            //When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (_, message, ct) =>
            //{
            //    if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
            //    {
            //        return;
            //    }

            //    var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

            //    await FindAndProduce(async () =>
            //            await snapshotManager.FindMatchingSnapshot(
            //                GetNisCode(municipalityDetail),
            //                message.Message.Provenance.Timestamp,
            //                throwStaleWhenGone: false,
            //                ct),
            //            ct);
            //});

            //When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<MunicipalityWasDrawn>>(async (_, message, ct) =>
            //{
            //    if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
            //    {
            //        return;
            //    }

            //    var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

            //    await FindAndProduce(async () =>
            //            await snapshotManager.FindMatchingSnapshot(
            //                GetNisCode(municipalityDetail),
            //                message.Message.Provenance.Timestamp,
            //                throwStaleWhenGone: false,
            //                ct),
            //            ct);
            //});

            When<Envelope<MunicipalityNameWasCleared>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }
                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (_, message, ct) =>
            {
                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityWasRegistered>>(async (_, message, ct) =>
            {
                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                {
                    return;
                }

                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (_, message, ct) =>
            {
                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            throwStaleWhenGone: false,
                            ct),
                        ct);
            });
        }

        private static string GetNisCode(MunicipalityDetail municipalityDetail)
        {
            return municipalityDetail.NisCode
                   ?? $"Niscode of MunicipalityDetail was unexpectedly null with municipalityId: '{municipalityDetail.MunicipalityId}'.";
        }

        private static MunicipalityDetail? GetMunicipalityDetail(LegacyContext legacyContext, Guid municipalityId)
        {
            var municipalityDetail = legacyContext.MunicipalityDetail.FirstOrDefault(m => m.MunicipalityId == municipalityId);
            if (municipalityDetail is null)
            {
                throw new InvalidOperationException(
                    $"Did not find MunicipalityDetail in Legacy projections for municipalityId '{municipalityId}'.");
            }
            return municipalityDetail;
        }

        private async Task FindAndProduce(Func<Task<OsloResult?>> findMatchingSnapshot, CancellationToken ct)
        {
            var result = await findMatchingSnapshot.Invoke();

            if (result != null)
            {
                await Produce(result.Identificator.Id, result.JsonContent, ct);
            }
        }

        private async Task Produce(string objectId, string jsonContent, CancellationToken cancellationToken = default)
        {
            var result = await KafkaProducer.Produce(_kafkaOptions, objectId, jsonContent, cancellationToken);
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Error + Environment.NewLine + result.ErrorReason); //TODO: create custom exception
            }
        }
    }
}
