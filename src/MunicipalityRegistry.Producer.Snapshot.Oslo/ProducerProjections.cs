namespace MunicipalityRegistry.Producer.Snapshot.Oslo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Oslo.SnapshotProducer;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Producer;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using MunicipalityRegistry.Projections.Legacy;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;

    [ConnectedProjectionName("Kafka producer snapshot oslo")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public sealed class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        public const string TopicKey = "MunicipalityTopic";

        private readonly IProducer _producer;

        public ProducerProjections(
            IProducer producer,
            ISnapshotManager snapshotManager,
            LegacyContext legacyContext,
            string osloNamespace)
        {
            _producer = producer;

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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
            //            message.Position,
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
            //            message.Position,
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
            //            message.Position,
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
            //            message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (_, message, ct) =>
            {
                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
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
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (_, message, ct) =>
            {
                var municipalityDetail = GetMunicipalityDetail(legacyContext, message.Message.MunicipalityId);

                await FindAndProduce(async () =>
                        await snapshotManager.FindMatchingSnapshot(
                            GetNisCode(municipalityDetail),
                            message.Message.Provenance.Timestamp,
                            null,
                            message.Position,
                            throwStaleWhenGone: false,
                            ct),
                    message.Position,
                    ct);
            });
        }

        private static string GetNisCode(MunicipalityDetail municipalityDetail)
        {
            return municipalityDetail.NisCode
                   ??
                   $"Niscode of MunicipalityDetail was unexpectedly null with municipalityId: '{municipalityDetail.MunicipalityId}'.";
        }

        private static MunicipalityDetail GetMunicipalityDetail(LegacyContext legacyContext, Guid municipalityId)
        {
            var municipalityDetail =
                legacyContext.MunicipalityDetail.FirstOrDefault(m => m.MunicipalityId == municipalityId);
            if (municipalityDetail is null)
            {
                throw new InvalidOperationException(
                    $"Did not find MunicipalityDetail in Legacy projections for municipalityId '{municipalityId}'.");
            }

            return municipalityDetail;
        }

        private async Task FindAndProduce(
            Func<Task<OsloResult?>> findMatchingSnapshot,
            long storePosition,
            CancellationToken ct)
        {
            var result = await findMatchingSnapshot.Invoke();

            if (result != null)
            {
                await Produce(result.Identificator.Id, result.Identificator.ObjectId, result.JsonContent, storePosition, ct);
            }
        }

        private async Task Produce(
            string puri,
            string objectId,
            string jsonContent,
            long storePosition,
            CancellationToken cancellationToken = default)
        {
            var result = await _producer.Produce(
                new MessageKey(puri),
                jsonContent,
                new List<MessageHeader>
                    { new MessageHeader(MessageHeader.IdempotenceKey, $"{objectId}-{storePosition.ToString()}") },
                cancellationToken);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Error + Environment.NewLine +
                                                    result.ErrorReason);
            }
        }
    }
}
