namespace MunicipalityRegistry.Projections.WmsWfs
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using MunicipalityList;

    public class WmsWfsContext : RunnerDbContext<WmsWfsContext>
    {
        public override string ProjectionStateSchema => Schema.WmsWfs;

        public DbSet<MunicipalityListItem> MunicipalityList { get; set; }
        public DbSet<MunicipalityDetail.MunicipalityDetail> MunicipalityDetail { get; set; }
        public DbSet<MunicipalityName.MunicipalityName> MunicipalityName { get; set; }
        // This needs to be here to please EF
        public WmsWfsContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public WmsWfsContext(DbContextOptions<WmsWfsContext> options)
            : base(options) { }
    }
}
