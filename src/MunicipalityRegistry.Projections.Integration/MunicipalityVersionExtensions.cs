namespace MunicipalityRegistry.Projections.Integration
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.EntityFrameworkCore;

    public static class MunicipalityVersionExtensions
    {
        public static async Task CreateNewMunicipalityVersion<T>(
            this IntegrationContext context,
            Guid municipalityId,
            Envelope<T> message,
            Action<MunicipalityVersion> applyEventInfoOn,
            CancellationToken ct) where T : IHasProvenance, IMessage
        {
            var municipalityVersion = await context.LatestPosition(municipalityId, ct);

            if (municipalityVersion is null)
            {
                throw DatabaseItemNotFound(municipalityId);
            }

            var provenance = message.Message.Provenance;

            var newMunicipalityVersion = municipalityVersion.CloneAndApplyEventInfo(
                message.Position,
                provenance.Timestamp,
                applyEventInfoOn);

            await context
                .MunicipalityVersions
                .AddAsync(newMunicipalityVersion, ct);
        }

        private static async Task<MunicipalityVersion?> LatestPosition(
            this IntegrationContext context,
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

        private static ProjectionItemNotFoundException<MunicipalityVersionProjections> DatabaseItemNotFound(Guid municipalityId)
            => new(municipalityId.ToString("D"));
    }
}
