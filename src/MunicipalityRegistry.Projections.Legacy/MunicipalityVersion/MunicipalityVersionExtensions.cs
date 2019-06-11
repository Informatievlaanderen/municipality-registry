namespace MunicipalityRegistry.Projections.Legacy.MunicipalityVersion
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.EntityFrameworkCore;

    public static class MunicipalityVersionExtensions
    {
        public static async Task CreateNewMunicipalityVersion<T>(
            this LegacyContext context,
            Guid municipalityId,
            Envelope<T> message,
            Action<MunicipalityVersion> applyEventInfoOn,
            CancellationToken ct) where T : IHasProvenance
        {
            var municipalityVersion = await context.LatestPosition(municipalityId, ct);

            if (municipalityVersion == null)
                throw DatabaseItemNotFound(municipalityId);

            var provenance = message.Message.Provenance;

            var newMunicipalityVersion = municipalityVersion.CloneAndApplyEventInfo(
                message.Position,
                applyEventInfoOn);

            newMunicipalityVersion.ApplyProvenance(provenance);

            await context
                .MunicipalityVersions
                .AddAsync(newMunicipalityVersion, ct);
        }

        public static async Task<MunicipalityVersion> LatestPosition(
            this LegacyContext context,
            Guid municipalityId,
            CancellationToken ct)
            => context
                   .MunicipalityVersions
                   .Local
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefault()
               ?? await context
                   .MunicipalityVersions
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefaultAsync(ct);

        public static void ApplyProvenance(
            this MunicipalityVersion municipalityVersion,
            ProvenanceData provenance)
        {
            municipalityVersion.Organisation = provenance.Organisation;
            municipalityVersion.Application = provenance.Application;
            municipalityVersion.Reason = provenance.Reason;
            municipalityVersion.Modification = provenance.Modification;
            municipalityVersion.Operator = provenance.Operator;
            municipalityVersion.VersionTimestamp = provenance.Timestamp;
        }

        private static ProjectionItemNotFoundException<MunicipalityVersionProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityVersionProjections>(municipalityId.ToString("D"));
    }
}
