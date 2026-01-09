namespace MunicipalityRegistry.Projections.Feed.MunicipalityFeed
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.EntityFrameworkCore;

    public static class MunicipalityFeedExtensions
    {
        public static async Task<int> CalculatePage(this FeedContext context)
        {
            if (!context.MunicipalityFeed.Any())
                return 1;

            var maxPage = await context.MunicipalityFeed.MaxAsync(x => x.Page);

            var pageItems = await context.MunicipalityFeed.CountAsync(x => x.Page == maxPage);
            return pageItems >= MunicipalityFeedProjections.MaxPageSize ? maxPage + 1 : maxPage;
        }
    }
}
