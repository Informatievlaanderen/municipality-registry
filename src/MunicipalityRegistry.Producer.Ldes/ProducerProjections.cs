namespace MunicipalityRegistry.Producer.Ldes
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Producer;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using Newtonsoft.Json;
    using NodaTime;

    [ConnectedProjectionName("Kafka producer LDES")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public sealed class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        public const string TopicKey = "MunicipalityTopic";

        private readonly IProducer _producer;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly string _osloNamespace;

        public ProducerProjections(
            IProducer producer,
            string osloNamespace,
            JsonSerializerSettings jsonSerializerSettings)
        {
            _producer = producer;
            _jsonSerializerSettings = jsonSerializerSettings;
            _osloNamespace = osloNamespace.Trim('/');

            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .Municipalities
                    .AddAsync(
                        new MunicipalityDetail
                        {
                            MunicipalityId = message.Message.MunicipalityId,
                            NisCode = message.Message.NisCode,
                            Status = MunicipalityStatus.Proposed,
                            VersionTimestamp = message.Message.Provenance.Timestamp
                        }, ct);

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.NisCode = message.Message.NisCode;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.NisCode = message.Message.NisCode;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, null);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, null);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.AddOfficialLanguage(message.Message.Language);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.RemoveOfficialLanguage(message.Message.Language);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.AddFacilitiesLanguage(message.Message.Language);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.RemoveFacilitiesLanguage(message.Message.Language);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Current;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Current;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                if (message.Message.Provenance.Timestamp.ToDateTimeOffset().Year < 2020)
                    return;

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityWasMerged>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.IsRemoved = true;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);

                await Produce(context, message.Message.MunicipalityId, message.Position, ct);
            });

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
        }

        private static MunicipalityDetail GetMunicipalityLdes(ProducerContext producerContext, Guid municipalityId)
        {
            var municipalityLdes =
                producerContext.Municipalities.Find(municipalityId);
            if (municipalityLdes is null)
            {
                throw new InvalidOperationException(
                    $"Did not find MunicipalityLdes in Legacy projections for municipalityId '{municipalityId}'.");
            }

            return municipalityLdes;
        }

        private static void UpdateNameByLanguage(MunicipalityDetail municipality, Language? language, string name)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipality.NameDutch = name;
                    break;

                case Language.French:
                    municipality.NameFrench = name;
                    break;

                case Language.German:
                    municipality.NameGerman = name;
                    break;

                case Language.English:
                    municipality.NameEnglish = name;
                    break;
            }
        }

        private static void UpdateVersionTimestamp(MunicipalityDetail municipality, Instant versionTimestamp)
            => municipality.VersionTimestamp = versionTimestamp;

        private static async Task DoNothing()
        {
            await Task.Yield();
        }

        private async Task Produce(
            ProducerContext context,
            Guid municipalityId,
            long storePosition,
            CancellationToken cancellationToken = default)
        {
            var municipalityDetail = GetMunicipalityLdes(context, municipalityId);
            if(!RegionFilter.IsFlemishRegion(municipalityDetail.NisCode))
                return;

            var municipalityLdes = new MunicipalityLdes(municipalityDetail, _osloNamespace);

            await Produce(
                $"{_osloNamespace}/{municipalityDetail.NisCode}",
                municipalityDetail.NisCode,
                JsonConvert.SerializeObject(municipalityLdes, _jsonSerializerSettings),
                storePosition,
                cancellationToken);
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
