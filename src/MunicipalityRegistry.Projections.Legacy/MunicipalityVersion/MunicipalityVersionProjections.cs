namespace MunicipalityRegistry.Projections.Legacy.MunicipalityVersion
{
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    // TODO: Make MunicipalityVersionProjections and MunicipalitySyndicationProjections more consistent
    public class MunicipalityVersionProjections : ConnectedProjection<LegacyContext>
    {
        public MunicipalityVersionProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(Handle);
            When<Envelope<MunicipalityBecameCurrent>>(Handle);
            When<Envelope<MunicipalityNameWasCleared>>(Handle);
            When<Envelope<MunicipalityNameWasCorrected>>(Handle);
            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(Handle);
            When<Envelope<MunicipalityNisCodeWasCorrected>>(Handle);
            When<Envelope<MunicipalityNisCodeWasDefined>>(Handle);
            When<Envelope<MunicipalityWasCorrectedToCurrent>>(Handle);
            When<Envelope<MunicipalityWasCorrectedToRetired>>(Handle);
            When<Envelope<MunicipalityWasNamed>>(Handle);
            When<Envelope<MunicipalityWasRetired>>(Handle);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityWasRegistered> message, CancellationToken ct)
        {
            var municipalityVersion = new MunicipalityVersion
            {
                MunicipalityId = message.Message.MunicipalityId,
                NisCode = message.Message.NisCode,
                Position = message.Position,
            };

            ApplyProvenance(municipalityVersion, message.Message.Provenance);

            await context
                .MunicipalityVersions
                .AddAsync(municipalityVersion, ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityNisCodeWasDefined> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => version.NisCode = message.Message.NisCode,
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityNisCodeWasCorrected> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => version.NisCode = message.Message.NisCode,
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityWasNamed> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => SetName(version, message.Message.Language, message.Message.Name),
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityNameWasCleared> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => ClearName(version, message.Message.Language),
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityNameWasCorrected> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => SetName(version, message.Message.Language, message.Message.Name),
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityNameWasCorrectedToCleared> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => ClearName(version, message.Message.Language),
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityBecameCurrent> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => { version.Status = MunicipalityStatus.Current; },
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityWasCorrectedToCurrent> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => { version.Status = MunicipalityStatus.Current; },
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityWasRetired> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => { version.Status = MunicipalityStatus.Retired; },
                ct);
        }

        private static async Task Handle(LegacyContext context, Envelope<MunicipalityWasCorrectedToRetired> message, CancellationToken ct)
        {
            await CreateNewMunicipalityVersion(
                context,
                message.Message.MunicipalityId,
                message.Position,
                message.Message.Provenance,
                version => { version.Status = MunicipalityStatus.Retired; },
                ct);
        }

        private static async Task CreateNewMunicipalityVersion(
            LegacyContext context,
            Guid messageMunicipalityId,
            long messagePosition,
            ProvenanceData messageProvenance,
            Action<MunicipalityVersion> applyEventInfoOn,
            CancellationToken ct)
        {
            var municipality = await context.LatestPosition(messageMunicipalityId, ct);

            if (municipality == null)
                throw DatabaseItemNotFound(messageMunicipalityId);

            var municipalityVersion = new MunicipalityVersion
            {
                MunicipalityId = municipality.MunicipalityId,
                NisCode = municipality.NisCode,
                NameDutch = municipality.NameDutch,
                NameFrench = municipality.NameFrench,
                NameGerman = municipality.NameGerman,
                NameEnglish = municipality.NameEnglish,
                Status = municipality.Status,
                Position = messagePosition,
            };

            applyEventInfoOn(municipalityVersion);

            ApplyProvenance(municipalityVersion, messageProvenance);

            await context
                .MunicipalityVersions
                .AddAsync(municipalityVersion, ct);
        }

        private static void SetName(MunicipalityVersion municipality, Language language, string name)
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
        }

        private static void ClearName(MunicipalityVersion municipality, Language language)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipality.NameDutch = string.Empty;
                    break;
                case Language.French:
                    municipality.NameFrench = string.Empty;
                    break;
                case Language.German:
                    municipality.NameGerman = string.Empty;
                    break;
                case Language.English:
                    municipality.NameEnglish = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }

        private static void ApplyProvenance(MunicipalityVersion municipalityVersion, ProvenanceData provenance)
        {
            municipalityVersion.Organisation = provenance.Organisation;
            municipalityVersion.Application = provenance.Application;
            municipalityVersion.Plan = provenance.Plan;
            municipalityVersion.Modification = provenance.Modification;
            municipalityVersion.Operator = provenance.Operator;
            municipalityVersion.VersionTimestamp = provenance.Timestamp;
        }

        private static ProjectionItemNotFoundException<MunicipalityVersionProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityVersionProjections>(municipalityId.ToString("D"));
    }
}
