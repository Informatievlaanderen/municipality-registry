namespace MunicipalityRegistry.Projections.Feed.MunicipalityFeed
{
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.ChangeFeed;
    using Microsoft.EntityFrameworkCore;

    public static class MunicipalityFeedExtensions
    {
        public static async Task<int> CalculatePage(this FeedContext context, int maxPageSize = ChangeFeedService.DefaultMaxPageSize)
        {
            if (!await context.MunicipalityFeed.AnyAsync())
            {
                return 1;
            }

            var maxPage = await context.MunicipalityFeed.MaxAsync(x => x.Page);
            var dbCount = await context.MunicipalityFeed.CountAsync(x => x.Page == maxPage);

            // Count pending (unsaved) items in the change tracker assigned to the current max page
            // This fixes the issue where multiple items added in the same batch would all get the same page
            var localCount = context.MunicipalityFeed.Local
                .Count(x => x.Page == maxPage && context.Entry(x).State == EntityState.Added);

            var totalCount = dbCount + localCount;

            return totalCount >= maxPageSize ? maxPage + 1 : maxPage;
        }
    }
}
