namespace MunicipalityRegistry.Projections.WmsWfs.MunicipalityName
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityNameExtensions
    {
        public static async Task<MunicipalityName> FindAndUpdateMunicipalityName(
            this WmsWfsContext context,
            Guid municipalityId,
            Action<MunicipalityName> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityName
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            return municipality;
        }

        private static ProjectionItemNotFoundException<MunicipalityNameProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityNameProjections>(municipalityId.ToString("D"));
    }
}
