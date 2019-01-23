namespace MunicipalityRegistry.Projections.Extract.MunicipalityExtract
{
    using System;
    using System.Text;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using NodaTime;

    public class MunicipalityExtractProjections : ConnectedProjection<ExtractContext>
    {
        // TODO: Probably need to get these from enums and data vlaanderen from config
        private const string InUse = "InGebruik";
        private const string Retired = "Gehistoreerd";
        private const string IdUri = "https://data.vlaanderen.be/id/gemeente";

        private readonly Encoding _encoding;

        public MunicipalityExtractProjections(Encoding encoding)
        {
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
                            id = { Value = $"{IdUri}/{message.Message.NisCode}" },
                            versie = { Value = message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset().DateTime }
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
                        UpdateGemeenteNm(municipality, message.Message.Language, message.Message.Name);
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
                        UpdateGemeenteNm(municipality, message.Message.Language, message.Message.Name);
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
                        UpdateGemeenteNm(municipality, message.Message.Language, null);
                        UpdateVersie(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality => UpdateVersie(municipality, message.Message.Provenance.Timestamp),
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality => UpdateVersie(municipality, message.Message.Provenance.Timestamp),
                    ct);
            });

            When<Envelope<MunicipalityFacilitiesLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality => UpdateVersie(municipality, message.Message.Provenance.Timestamp),
                    ct);
            });


            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality => UpdateVersie(municipality, message.Message.Provenance.Timestamp),
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityExtract(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateGemeenteNm(municipality, message.Message.Language, null);
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
        }

        private void UpdateGemeenteNm(MunicipalityExtractItem municipality, Language language, string name)
            => UpdateRecord(municipality, record =>
            {
                if (language == Language.Dutch)
                    record.gemeentenm.Value = name;
                else if (record.gemeentenm.Value == null)
                    record.gemeentenm.Value = name;
            });

        private void UpdateStatus(MunicipalityExtractItem municipality, string status)
            => UpdateRecord(municipality, record => record.status.Value = status);

        private void UpdateId(MunicipalityExtractItem municipality, string id)
            => UpdateRecord(municipality, record =>
            {
                record.id.Value = $"{IdUri}/{id}";
                record.gemeenteid.Value = id;
            });

        private void UpdateVersie(MunicipalityExtractItem municipality, Instant timestamp)
            => UpdateRecord(municipality, record => record.versie.Value = timestamp.ToBelgianDateTimeOffset().DateTime);

        private void UpdateRecord(MunicipalityExtractItem municipality, Action<MunicipalityDbaseRecord> updateFunc)
        {
            var record = new MunicipalityDbaseRecord();
            record.FromBytes(municipality.DbaseRecord, _encoding);

            updateFunc(record);

            municipality.DbaseRecord = record.ToBytes(_encoding);
        }
    }
}
