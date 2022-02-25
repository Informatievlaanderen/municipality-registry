namespace MunicipalityRegistry.Projections.Wms
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public class WmsContextMigrationFactory : RunnerDbContextMigrationFactory<WmsContext>
    {
        public WmsContextMigrationFactory()
            : base("WmsProjectionsAdmin", HistoryConfiguration) { }

        private static MigrationHistoryConfiguration HistoryConfiguration =>
            new MigrationHistoryConfiguration
            {
                Schema = Schema.Wms,
                Table = MigrationTables.Wms
            };

        protected override WmsContext CreateContext(DbContextOptions<WmsContext> migrationContextOptions)
            => new WmsContext(migrationContextOptions);
    }
}
