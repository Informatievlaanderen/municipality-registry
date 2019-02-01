namespace MunicipalityRegistry.Projections.Legacy
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;

    public class MigrationsHelper : RunnerDbContextMigrationHelper<LegacyContext>
    {
        public MigrationsHelper(string connectionString, ILoggerFactory loggerFactory) :
            base(
                connectionString,
                new HistoryConfiguration { Schema = Schema.Legacy, Table = MigrationTables.Legacy},
                loggerFactory) { }

        protected override LegacyContext CreateContext(DbContextOptions<LegacyContext> migrationContextOptions)
            => new LegacyContext(migrationContextOptions);
    }
}
