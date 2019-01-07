namespace MunicipalityRegistry.Projections.Extract
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MunicipalityExtractItemConfiguration : IEntityTypeConfiguration<MunicipalityExtractItem>
    {
        public const string TableName = "Municipality";

        public void Configure(EntityTypeBuilder<MunicipalityExtractItem> builder)
        {
            builder.ToTable(TableName, Schema.Extract)
                .HasKey(p => p.MunicipalityId)
                .ForSqlServerIsClustered(false);

            builder.Property(p => p.NisCode);
            builder.Property(p => p.DbaseRecord);

            builder.HasIndex(p => p.NisCode).ForSqlServerIsClustered();
        }
    }
}
