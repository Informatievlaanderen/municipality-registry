namespace MunicipalityRegistry.Projections.Extract.MunicipalityExtract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityExtractExtensions
    {
        public static async Task<MunicipalityExtractItem> FindAndUpdateMunicipalityExtract(
            this ExtractContext context,
            Guid municipalityId,
            Action<MunicipalityExtractItem> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityExtract
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            return municipality;
        }

        private static ProjectionItemNotFoundException<MunicipalityExtractProjection> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityExtractProjection>(municipalityId.ToString("D"));
    }
}
