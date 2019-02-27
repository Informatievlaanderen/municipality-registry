namespace MunicipalityRegistry.Projections.Extract
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class ExtractContextMigrationsHelper : RunnerDbContextMigrationHelper<ExtractContext>
    {
        public ExtractContextMigrationsHelper(string connectionString, ILoggerFactory loggerFactory) :
            base(
                connectionString,
                new HistoryConfiguration { Schema = Schema.Extract, Table = MigrationTables.Extract },
                loggerFactory)
        { }

        protected override ExtractContext CreateContext(DbContextOptions<ExtractContext> migrationContextOptions)
            => new ExtractContext(migrationContextOptions);
    }
}
