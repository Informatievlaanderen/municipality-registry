namespace MunicipalityRegistry.Projections.WmsWfs
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public class WmsWfsContextMigrationFactory : RunnerDbContextMigrationFactory<WmsWfsContext>
    {
        public WmsWfsContextMigrationFactory()
            : base("WmsWfsProjectionsAdmin", HistoryConfiguration) { }

        private static MigrationHistoryConfiguration HistoryConfiguration =>
            new MigrationHistoryConfiguration
            {
                Schema = Schema.WmsWfs,
                Table = MigrationTables.WmsWfs
            };

        protected override WmsWfsContext CreateContext(DbContextOptions<WmsWfsContext> migrationContextOptions)
            => new WmsWfsContext(migrationContextOptions);
    }
}
