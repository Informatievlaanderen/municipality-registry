namespace MunicipalityRegistry.Projections.Legacy
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using MunicipalityList;
    using MunicipalitySyndication;

    public class LegacyContext : RunnerDbContext<LegacyContext>
    {
        public override string ProjectionStateSchema => Schema.Legacy;

        public DbSet<MunicipalityListItem> MunicipalityList { get; set; }
        public DbSet<MunicipalityDetail.MunicipalityDetail> MunicipalityDetail { get; set; }
        public DbSet<MunicipalityVersion.MunicipalityVersion> MunicipalityVersions { get; set; }
        public DbSet<MunicipalityName.MunicipalityName> MunicipalityName { get; set; }
        public DbSet<MunicipalitySyndicationItem> MunicipalitySyndication { get; set; }

        // This needs to be here to please EF
        public LegacyContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public LegacyContext(DbContextOptions<LegacyContext> options)
            : base(options) { }
    }
}
