namespace MunicipalityRegistry.Projections.Integration
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Microsoft.EntityFrameworkCore;
    using MunicipalityRegistry.Infrastructure;

    public class IntegrationContext : RunnerDbContext<IntegrationContext>
    {
        public override string ProjectionStateSchema => Schema.Integration;

        public DbSet<MunicipalityLatestItem> MunicipalityLatestItems => Set<MunicipalityLatestItem>();
        public DbSet<MunicipalityVersion> MunicipalityVersions => Set<MunicipalityVersion>();

        public DbSet<MunicipalityGeometry> MunicipalityGeometries => Set<MunicipalityGeometry>();

        // This needs to be here to please EF
        public IntegrationContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public IntegrationContext(DbContextOptions<IntegrationContext> options)
            : base(options) { }
    }
}
