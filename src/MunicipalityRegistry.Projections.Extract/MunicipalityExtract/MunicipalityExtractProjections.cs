namespace MunicipalityRegistry.Projections.Extract.MunicipalityExtract
{
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using NodaTime;
    using System;
    using System.Linq;
    using System.Text;
    using Microsoft.Extensions.Options;

    public class MunicipalityExtractProjections : ConnectedProjection<ExtractContext>
    {
        // TODO: Probably need to get these from enums from config
        private const string InUse = "InGebruik";
        private const string Retired = "Gehistoreerd";

        private readonly ExtractConfig _extractConfig;
        private readonly Encoding _encoding;

        public MunicipalityExtractProjections(IOptions<ExtractConfig> extractConfig, Encoding encoding)
        {
            _extractConfig = extractConfig.Value;
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));

            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .MunicipalityExtract
                    .AddAsync(new MunicipalityExtractItem
                    {
                        MunicipalityId = message.Message.MunicipalityId,
                        NisCode = message.Message.NisCode,
                        DbaseRecord = new MunicipalityDbaseRecord
                        {
                            gemeenteid = { Value = message.Message.NisCode },
                            id = { Value = $"{_extractConfig.DataVlaanderenNamespace}/{message.Message.NisCode}" },
                            versieid = { Value = message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset().DateTime }
                        }.ToBytes(_encoding)
                    }, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateId(municipality, message.Message.NisCode);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.NisCode = message.Message.NisCode;
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateName(municipality, message.Message.Language, message.Message.Name);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateName(municipality, message.Message.Language, message.Message.Name);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateName(municipality, message.Message.Language, null);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.AddOfficialLanguage(message.Message.Language);
                        UpdateRecord(municipality, record => UpdateGemeentenm(municipality, record));
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.RemoveOfficialLanguage(message.Message.Language);
                        UpdateRecord(municipality, record => UpdateGemeentenm(municipality, record));
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateName(municipality, message.Message.Language, null);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateStatus(municipality, InUse);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateStatus(municipality, InUse);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateStatus(municipality, Retired);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateStatus(municipality, Retired);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilitiesLanguageWasAdded>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
        }

        private void UpdateName(MunicipalityExtractItem municipality, Language language, string name)
            => UpdateRecord(municipality, record =>
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
                    default:
                        throw new ArgumentOutOfRangeException(nameof(language), language, null);
                }

                UpdateGemeentenm(municipality, record);
            });

        private static void UpdateGemeentenm(MunicipalityExtractItem municipality, MunicipalityDbaseRecord record)
        {
            if (municipality.OfficialLanguages.Any())
            {
                switch (municipality.OfficialLanguages.First())
                {
                    case Language.Dutch:
                        record.gemeentenm.Value = municipality.NameDutch;
                        break;
                    case Language.French:
                        record.gemeentenm.Value = municipality.NameFrench;
                        break;
                    case Language.German:
                        record.gemeentenm.Value = municipality.NameGerman;
                        break;
                    case Language.English:
                        record.gemeentenm.Value = municipality.NameEnglish;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateStatus(MunicipalityExtractItem municipality, string status)
            => UpdateRecord(municipality, record => record.status.Value = status);

        private void UpdateId(MunicipalityExtractItem municipality, string id)
            => UpdateRecord(municipality, record =>
            {
                record.id.Value = $"{_extractConfig.DataVlaanderenNamespace}/{id}";
                record.gemeenteid.Value = id;
            });

        private void UpdateVersie(MunicipalityExtractItem municipality, Instant timestamp)
            => UpdateRecord(municipality, record => record.versieid.Value = timestamp.ToBelgianDateTimeOffset().DateTime);

        private void UpdateRecord(MunicipalityExtractItem municipality, Action<MunicipalityDbaseRecord> updateFunc)
        {
            var record = new MunicipalityDbaseRecord();
            record.FromBytes(municipality.DbaseRecord, _encoding);

            updateFunc(record);

            municipality.DbaseRecord = record.ToBytes(_encoding);
        }

        private static void DoNothing() { }
    }
}
