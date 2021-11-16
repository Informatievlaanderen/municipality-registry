namespace MunicipalityRegistry.Projections.QueuePublisher
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public class QueuePublisherContextMigrationFactory : RunnerDbContextMigrationFactory<QueuePublisherContext>
    {
        public QueuePublisherContextMigrationFactory()
            : base("QueuePublisherProjectionsAdmin", HistoryConfiguration) { }

        private static MigrationHistoryConfiguration HistoryConfiguration =>
            new MigrationHistoryConfiguration
            {
                Schema = Schema.QueuePublisher,
                Table = MigrationTables.QueuePublisher
            };

        protected override QueuePublisherContext CreateContext(DbContextOptions<QueuePublisherContext> migrationContextOptions)
            => new QueuePublisherContext(migrationContextOptions);
    }
}
