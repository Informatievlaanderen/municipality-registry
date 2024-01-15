namespace MunicipalityRegistry.Projections.Integration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityLatestItemExtensions
    {
            public static async Task<MunicipalityLatestItem> FindAndUpdateMunicipality(this IntegrationContext context,
                Guid municipalityId,
                Action<MunicipalityLatestItem> updateFunc,
                CancellationToken ct)
            {
                var municipality = await context
                    .MunicipalityLatestItems
                    .FindAsync(municipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(municipalityId);

                updateFunc(municipality);

                return municipality;
            }

            private static ProjectionItemNotFoundException<MunicipalityLatestItemProjections> DatabaseItemNotFound(Guid municipalityId)
                => new ProjectionItemNotFoundException<MunicipalityLatestItemProjections>(municipalityId.ToString("D"));
    }
}
