namespace MunicipalityRegistry.Projections.WmsWfs.MunicipalityList
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityListExtensions
    {
        public static async Task<MunicipalityListItem> FindAndUpdateMunicipalityListItem(
            this WmsWfsContext context,
            Guid municipalityId,
            Action<MunicipalityListItem> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityList
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            return municipality;
        }

        private static ProjectionItemNotFoundException<MunicipalityListProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityListProjections>(municipalityId.ToString("D"));
    }
}
