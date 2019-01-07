namespace MunicipalityRegistry.Projections.Legacy.MunicipalityVersion
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public static class MunicipalityVersionExtensions
    {
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
    }
}
