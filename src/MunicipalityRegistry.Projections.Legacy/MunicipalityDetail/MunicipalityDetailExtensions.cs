namespace MunicipalityRegistry.Projections.Legacy.MunicipalityDetail
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityDetailExtensions
    {
        public static async Task<MunicipalityDetail> FindAndUpdateMunicipalityDetail(
            this LegacyContext context,
            Guid municipalityId,
            Action<MunicipalityDetail> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityDetail
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            return municipality;
        }

        private static ProjectionItemNotFoundException<MunicipalityDetailProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityDetailProjections>(municipalityId.ToString("D"));
    }
}
