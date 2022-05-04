namespace MunicipalityRegistry.Projections.Legacy.MunicipalityList
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityListExtensions
    {
        public static async Task<MunicipalityListItem> FindAndUpdateMunicipalityListItem(
            this LegacyContext context,
            Guid municipalityId,
            Action<MunicipalityListItem> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityList
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
            {
                throw DatabaseItemNotFound(municipalityId);
            }

            updateFunc(municipality);

            return municipality;
        }

        public static IQueryable<MunicipalityListItem> FlemishMunicipalities(this IQueryable<MunicipalityListItem> municipalities) => municipalities
            .ToList()
            .Where(x => Be.Vlaanderen.Basisregisters.GrAr.Legacy.RegionFilter.IsFlemishRegion(x.NisCode))
            .AsQueryable();

        private static ProjectionItemNotFoundException<MunicipalityListProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityListProjections>(municipalityId.ToString("D"));
    }
}
