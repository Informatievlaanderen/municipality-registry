namespace MunicipalityRegistry.Projections.Extract
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using NodaTime;

    public class MunicipalityExtractProjection : ConnectedProjection<ExtractContext>
    {
        // TODO: Probably need to get these from enums and data vlaanderen from config
        private const string InUse = "InGebruik";
        private const string Retired = "Gehistoreerd";
        private const string IdUri = "http://data.vlaanderen.be/id/gemeente";

        private readonly Encoding _encoding;

        public MunicipalityExtractProjection(Encoding encoding)
        {
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));

            When<Envelope<MunicipalityWasRegistered>>(Handle);
            When<Envelope<MunicipalityNisCodeWasDefined>>(Handle);
            When<Envelope<MunicipalityNisCodeWasCorrected>>(Handle);
            When<Envelope<MunicipalityWasNamed>>(Handle);
            When<Envelope<MunicipalityNameWasCorrected>>(Handle);
            When<Envelope<MunicipalityNameWasCleared>>(Handle);
            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(Handle);
            When<Envelope<MunicipalityBecameCurrent>>(Handle);
            When<Envelope<MunicipalityWasCorrectedToCurrent>>(Handle);
            When<Envelope<MunicipalityWasRetired>>(Handle);
            When<Envelope<MunicipalityWasCorrectedToRetired>>(Handle);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityWasRegistered> envelope, CancellationToken ct)
        {
            await context
                .MunicipalityExtract
                .AddAsync(new MunicipalityExtractItem
                {
                    MunicipalityId = envelope.Message.MunicipalityId,
                    NisCode = envelope.Message.NisCode,
                    DbaseRecord = new MunicipalityDbaseRecord
                    {
                        gemeenteid = { Value = envelope.Message.NisCode },
                        id = { Value = $"{IdUri}/{envelope.Message.NisCode}" },
                        versie = { Value = envelope.Message.Provenance.Timestamp.ToBelgianDateTimeOffset().DateTime }
                    }.ToBytes(_encoding)
                }, ct);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityBecameCurrent> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            UpdateStatus(municipality, InUse);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityWasCorrectedToCurrent> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            UpdateStatus(municipality, InUse);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityWasRetired> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            UpdateStatus(municipality, Retired);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityWasCorrectedToRetired> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            UpdateStatus(municipality, Retired);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityNameWasCorrectedToCleared> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            UpdateGemeenteNm(municipality, message.Message.Language, null);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityNameWasCleared> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            UpdateGemeenteNm(municipality, message.Message.Language, null);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityNameWasCorrected> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            UpdateGemeenteNm(municipality, message.Message.Language, message.Message.Name);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityWasNamed> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            UpdateGemeenteNm(municipality, message.Message.Language, message.Message.Name);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityNisCodeWasCorrected> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            municipality.NisCode = message.Message.NisCode;
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private async Task Handle(ExtractContext context, Envelope<MunicipalityNisCodeWasDefined> message, CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(message.Message.MunicipalityId);

            municipality.NisCode = message.Message.NisCode;

            UpdateId(municipality, message.Message.NisCode);
            UpdateVersie(municipality, message.Message.Provenance.Timestamp);
        }

        private static ProjectionItemNotFoundException<MunicipalityExtractProjection> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityExtractProjection>(municipalityId.ToString("D"));

        private void UpdateGemeenteNm(MunicipalityExtractItem municipality, Language language, string name)
        {
            var record = new MunicipalityDbaseRecord();
            record.FromBytes(municipality.DbaseRecord, _encoding);

            if (language == Language.Dutch)
                record.gemeentenm.Value = name;
            else if (record.gemeentenm.Value == null)
                record.gemeentenm.Value = name;

            municipality.DbaseRecord = record.ToBytes(_encoding);
        }

        private void UpdateStatus(MunicipalityExtractItem municipality, string status)
        {
            var record = new MunicipalityDbaseRecord();
            record.FromBytes(municipality.DbaseRecord, _encoding);
            record.status.Value = status;
            municipality.DbaseRecord = record.ToBytes(_encoding);
        }

        private void UpdateId(MunicipalityExtractItem municipality, string id)
        {
            var record = new MunicipalityDbaseRecord();
            record.FromBytes(municipality.DbaseRecord, _encoding);
            record.id.Value = $"{IdUri}/{id}";
            record.gemeenteid.Value = id;
            municipality.DbaseRecord = record.ToBytes(_encoding);
        }

        private void UpdateVersie(MunicipalityExtractItem municipality, Instant timestamp)
        {
            var record = new MunicipalityDbaseRecord();
            record.FromBytes(municipality.DbaseRecord, _encoding);
            record.versie.Value = timestamp.ToBelgianDateTimeOffset().DateTime;
            municipality.DbaseRecord = record.ToBytes(_encoding);
        }
    }
}
