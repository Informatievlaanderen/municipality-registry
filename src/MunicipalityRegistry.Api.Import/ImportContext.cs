namespace MunicipalityRegistry.Api.Import
{
    using Microsoft.EntityFrameworkCore;

    public class ImportContext : DbContext
    {
        public DbSet<MunicipalityMerger> MunicipalityMergers => Set<MunicipalityMerger>();

        public ImportContext() { }

        public ImportContext(DbContextOptions<ImportContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MunicipalityMergerConfiguration());
        }
    }
}
