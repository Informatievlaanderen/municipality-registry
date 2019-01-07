namespace MunicipalityRegistry.Projections.Legacy.MunicipalityVersion
{
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;
    using System;

    public class MunicipalityVersion
    {
        public static string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public Guid? MunicipalityId { get; set; }
        public string NisCode { get; set; }

        public string NameDutch { get; set; }
        public string NameFrench { get; set; }
        public string NameGerman { get; set; }
        public string NameEnglish { get; set; }

        public MunicipalityStatus? Status { get; set; }

        public long Position { get; set; }

        public Instant? VersionTimestamp
        {
            get => VersionTimestampAsDateTimeOffset == null ? (Instant?)null : Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset.Value);
            set => VersionTimestampAsDateTimeOffset = value?.ToDateTimeOffset();
        }

        private DateTimeOffset? VersionTimestampAsDateTimeOffset { get; set; }

        public Application? Application { get; set; }
        public Modification? Modification { get; set; }
        public string Operator { get; set; }
        public Organisation? Organisation { get; set; }
        public Plan? Plan { get; set; }
    }

    public class MunicipalityVersionConfiguration : IEntityTypeConfiguration<MunicipalityVersion>
    {
        private const string TableName = "MunicipalityVersions";

        public void Configure(EntityTypeBuilder<MunicipalityVersion> builder)
        {
            builder.ToTable(TableName, Schema.Legacy)
                .HasKey(x => new { x.MunicipalityId, x.Position })
                .ForSqlServerIsClustered(false);

            builder.Property(x => x.Position);
            builder.Property(x => x.NisCode);
            builder.Property(x => x.NameDutch);
            builder.Property(x => x.NameFrench);
            builder.Property(x => x.NameGerman);
            builder.Property(x => x.NameEnglish);
            builder.Property(x => x.Status);

            builder.Property(MunicipalityVersion.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            builder.Property(x => x.Application);
            builder.Property(x => x.Modification);
            builder.Property(x => x.Operator);
            builder.Property(x => x.Organisation);
            builder.Property(x => x.Plan);

            builder.Ignore(x => x.VersionTimestamp);

            builder.HasIndex(x => x.NisCode)
                .ForSqlServerIsClustered();
        }
    }
}
