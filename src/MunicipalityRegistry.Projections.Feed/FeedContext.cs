namespace MunicipalityRegistry.Projections.Feed
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using MunicipalityFeed;
    using Newtonsoft.Json;

    public class FeedContext : RunnerDbContext<FeedContext>
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public override string ProjectionStateSchema => Schema.Feed;

        public DbSet<MunicipalityFeedItem> MunicipalityFeed { get; set; }

        public DbSet<MunicipalityDocument> MunicipalityDocuments { get; set; }

        // This needs to be here to please EF
        public FeedContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public FeedContext(DbContextOptions<FeedContext> options, JsonSerializerSettings jsonSerializerSettings)
            : base(options)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MunicipalityFeedConfiguration());
            modelBuilder.ApplyConfiguration(new MunicipalityDocumentConfiguration(_jsonSerializerSettings));
        }
    }
}
