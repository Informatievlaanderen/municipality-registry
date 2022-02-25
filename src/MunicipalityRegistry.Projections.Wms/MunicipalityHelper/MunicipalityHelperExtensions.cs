namespace MunicipalityRegistry.Projections.Wms.Municipality
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityHelperExtensions
    {
        public static async Task<MunicipalityHelper> FindAndUpdateMunicipalityHelper(
            this WmsContext context,
            Guid municipalityId,
            Action<MunicipalityHelper> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityHelper
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            return municipality;
        }

        private static ProjectionItemNotFoundException<MunicipalityHelperProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityHelperProjections>(municipalityId.ToString("D"));
    }
}
