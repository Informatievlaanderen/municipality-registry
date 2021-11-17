namespace MunicipalityRegistry.Projections.StreamPublisher
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public class StreamPublisherContextMigrationFactory : RunnerDbContextMigrationFactory<StreamPublisherContext>
    {
        public StreamPublisherContextMigrationFactory()
            : base("StreamPublisherProjectionsAdmin", HistoryConfiguration) { }

        private static MigrationHistoryConfiguration HistoryConfiguration =>
            new MigrationHistoryConfiguration
            {
                Schema = Schema.StreamPublisher,
                Table = MigrationTables.StreamPublisher
            };

        protected override StreamPublisherContext CreateContext(DbContextOptions<StreamPublisherContext> migrationContextOptions)
            => new StreamPublisherContext(migrationContextOptions);
    }
}
