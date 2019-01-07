namespace MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public static class MunicipalitySyndicationExtensions
    {
        public static async Task<MunicipalitySyndicationItem> LatestPosition(
            this LegacyContext context,
            Guid municipalityId,
            CancellationToken ct)
            => context
                   .MunicipalitySyndication
                   .Local
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefault()
               ?? await context
                   .MunicipalitySyndication
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefaultAsync(ct);
    }
}
