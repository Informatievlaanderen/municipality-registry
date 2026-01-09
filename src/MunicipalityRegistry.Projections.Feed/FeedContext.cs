namespace MunicipalityRegistry.Projections.Feed
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using MunicipalityFeed;

    public class FeedContext : RunnerDbContext<FeedContext>
    {
        public override string ProjectionStateSchema => Schema.Feed;

        public DbSet<MunicipalityFeedItem> MunicipalityFeed { get; set; }

        public DbSet<MunicipalityDocument> MunicipalityDocuments { get; set; }

        // This needs to be here to please EF
        public FeedContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public FeedContext(DbContextOptions<FeedContext> options)
            : base(options) { }
    }
}
