namespace MunicipalityRegistry.Projections.Legacy
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class LegacyContextMigrationHelper : RunnerDbContextMigrationHelper<LegacyContext>
    {
        public LegacyContextMigrationHelper(
            string connectionString,
            ILoggerFactory loggerFactory)
            : base(
                connectionString,
                new HistoryConfiguration
                {
                    Schema = Schema.Legacy,
                    Table = MigrationTables.Legacy
                },
                loggerFactory) { }

        protected override LegacyContext CreateContext(DbContextOptions<LegacyContext> migrationContextOptions) => new LegacyContext(migrationContextOptions);
    }
}
