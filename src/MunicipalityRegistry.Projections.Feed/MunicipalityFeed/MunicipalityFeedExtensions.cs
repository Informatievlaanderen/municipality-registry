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
            if (!context.MunicipalityFeed.Any())
                return 1;

            var maxPage = await context.MunicipalityFeed.MaxAsync(x => x.Page);

            var pageItems = await context.MunicipalityFeed.CountAsync(x => x.Page == maxPage);
            return pageItems >= maxPageSize ? maxPage + 1 : maxPage;
        }
    }
}
