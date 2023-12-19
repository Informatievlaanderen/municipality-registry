namespace MunicipalityRegistry.Projections.Integration
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Npgsql;
    using Microsoft.EntityFrameworkCore;
    using MunicipalityRegistry.Infrastructure;

    public class IntegrationContextMigrationFactory : NpgsqlRunnerDbContextMigrationFactory<IntegrationContext>
    {
        public IntegrationContextMigrationFactory()
            : base("IntegrationProjectionsAdmin", HistoryConfiguration) { }

        private static MigrationHistoryConfiguration HistoryConfiguration =>
            new MigrationHistoryConfiguration
            {
                Schema = Schema.Integration,
                Table = MigrationTables.Integration
            };

        protected override IntegrationContext CreateContext(DbContextOptions<IntegrationContext> migrationContextOptions)
            => new IntegrationContext(migrationContextOptions);
    }
}
