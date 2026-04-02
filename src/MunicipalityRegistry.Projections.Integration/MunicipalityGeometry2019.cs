namespace MunicipalityRegistry.Projections.Integration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MunicipalityRegistry.Infrastructure;
    using NetTopologySuite.Geometries;

    public class MunicipalityGeometry2019
    {
        public string NisCode { get; set; }
        public Geometry Geometry { get; set; }
        public Geometry GeometryLambert08 { get; set; }

        public MunicipalityGeometry2019()
        { }
    }

    public sealed class MunicipalityGeometry2019Configuration : IEntityTypeConfiguration<MunicipalityGeometry2019>
    {
        public void Configure(EntityTypeBuilder<MunicipalityGeometry2019> builder)
        {
            builder
                .ToTable("municipality_geometries_2019", Schema.Integration)
                .HasKey(x => x.NisCode);

            builder.Property(x => x.NisCode)
                .HasColumnName("nis_code")
                .HasMaxLength(5)
                .IsFixedLength()
                .ValueGeneratedNever();
            builder.Property(x => x.Geometry)
                .HasColumnName("geometry");

            builder.Property(x => x.GeometryLambert08)
                .HasColumnName("geometry_lambert08");

            builder.HasIndex(x => x.NisCode);
            builder.HasIndex(x => x.Geometry)
                .HasMethod("GIST");

            builder.HasIndex(x => x.GeometryLambert08)
                .HasMethod("GIST");
        }
    }
}
