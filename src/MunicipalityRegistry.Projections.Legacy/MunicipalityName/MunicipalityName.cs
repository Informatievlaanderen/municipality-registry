namespace MunicipalityRegistry.Projections.Legacy.MunicipalityName
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;

    /// <summary>
    /// BOSA
    /// </summary>
    public class MunicipalityName
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; }

        public string NameDutch { get; set; }
        public string NameDutchSearch { get; set; }

        public string NameFrench { get; set; }
        public string NameFrenchSearch { get; set; }

        public string NameGerman { get; set; }
        public string NameGermanSearch { get; set; }

        public string NameEnglish { get; set; }
        public string NameEnglishSearch { get; set; }

        public bool IsFlemishRegion { get; set; }

        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }
    }

    public class MunicipalityNameConfiguration : IEntityTypeConfiguration<MunicipalityName>
    {
        private const string TableName = "MunicipalityName";

        public void Configure(EntityTypeBuilder<MunicipalityName> builder)
        {
            builder.ToTable(TableName, Schema.Legacy)
                .HasKey(p => p.MunicipalityId)
                .ForSqlServerIsClustered(false);

            builder.Property(p => p.NisCode);
            builder.Property(p => p.NameDutch);
            builder.Property(p => p.NameDutchSearch);
            builder.Property(p => p.NameFrench);
            builder.Property(p => p.NameFrenchSearch);
            builder.Property(p => p.NameGerman);
            builder.Property(p => p.NameGermanSearch);
            builder.Property(p => p.NameEnglish);
            builder.Property(p => p.NameEnglishSearch);
            builder.Property(p => p.IsFlemishRegion);

            builder.Property(MunicipalityName.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            builder.Ignore(x => x.VersionTimestamp);

            builder.HasIndex(p => p.NisCode).ForSqlServerIsClustered();
            builder.HasIndex(MunicipalityName.VersionTimestampBackingPropertyName);
            builder.HasIndex(p => p.IsFlemishRegion);

            builder.HasIndex(p => p.NameDutchSearch);
            builder.HasIndex(p => p.NameFrenchSearch);
            builder.HasIndex(p => p.NameGermanSearch);
            builder.HasIndex(p => p.NameEnglishSearch);
        }
    }
}
