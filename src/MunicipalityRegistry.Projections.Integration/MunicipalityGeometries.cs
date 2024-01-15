namespace MunicipalityRegistry.Projections.Integration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MunicipalityRegistry.Infrastructure;
    using NetTopologySuite.Geometries;

    public class MunicipalityGeometry
    {
        public string NisCode { get; set; }
        public Geometry Geometry { get; set; }

        public MunicipalityGeometry()
        { }
    }

    public sealed class MunicipalityGeometryConfiguration : IEntityTypeConfiguration<MunicipalityGeometry>
    {
        public void Configure(EntityTypeBuilder<MunicipalityGeometry> builder)
        {
            builder
                .ToTable("municipality_geometries", Schema.Integration)
                .HasKey(x => x.NisCode);

            builder.Property(x => x.NisCode)
                .HasColumnName("nis_code")
                .HasMaxLength(5)
                .IsFixedLength()
                .ValueGeneratedNever();
            builder.Property(x => x.Geometry)
                .HasColumnName("geometry");

            builder.HasIndex(x => x.NisCode);
            builder.HasIndex(x => x.Geometry)
                .HasMethod("GIST");
        }
    }
}
