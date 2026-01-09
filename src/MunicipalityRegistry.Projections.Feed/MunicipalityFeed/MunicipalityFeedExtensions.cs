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
            var maxPage = await context.MunicipalityFeed
                .Select(x => x.Page)
                .DefaultIfEmpty(0)
                .MaxAsync();

            var pageItems = await context.MunicipalityFeed.CountAsync(x => x.Page == maxPage);
            return pageItems >= 100 ? maxPage + 1 : maxPage;
        }

        public static async Task CreateNewMunicipalityFeedItem<T>(
            this FeedContext context,
            Guid municipalityId,
            Envelope<T> message,
            Action<MunicipalityFeedItem> applyEventInfoOn,
            CancellationToken ct) where T : IHasProvenance, IMessage
        {
            var municipalityFeedItem = await context.LatestPosition(municipalityId, ct);

            if (municipalityFeedItem == null)
                throw DatabaseItemNotFound(municipalityId);

            var provenance = message.Message.Provenance;



            // newMunicipalityFeedItem.ApplyProvenance(provenance);
            // //newMunicipalityFeedItem.SetEventData(message.Message, message.EventName);
            //
            // await context
            //     .MunicipalityFeed
            //     .AddAsync(newMunicipalityFeedItem, ct);
        }

        public static async Task<MunicipalityFeedItem> LatestPosition(
            this FeedContext context,
            Guid municipalityId,
            CancellationToken ct)
            => context
                   .MunicipalityFeed
                   .Local
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefault()
               ?? await context
                   .MunicipalityFeed
                   .Where(x => x.MunicipalityId == municipalityId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefaultAsync(ct);

        public static void ApplyProvenance(
            this MunicipalityFeedItem item,
            ProvenanceData provenance)
        {
            item.Application = provenance.Application;
            item.Modification = provenance.Modification;
            item.Operator = provenance.Operator;
            item.Organisation = provenance.Organisation;
            item.Reason = provenance.Reason;
        }

        private static ProjectionItemNotFoundException<MunicipalityFeedProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityFeedProjections>(municipalityId.ToString("D"));
    }
}
