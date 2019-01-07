namespace MunicipalityRegistry.Projections.Legacy.MunicipalityList
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;

    public class MunicipalityListItem
    {
        public static string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public Guid? MunicipalityId { get; set; }
        public string NisCode { get; set; }

        public Language? PrimaryLanguage { get; set; }
        public Language? SecondaryLanguage { get; set; }

        public string DefaultName { get; set; }
        public string NameDutch { get; set; }
        public string NameFrench { get; set; }
        public string NameGerman { get; set; }
        public string NameEnglish { get; set; }

        public MunicipalityStatus? Status { get; set; }

        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }
    }

    public class MunicipalityListConfiguration : IEntityTypeConfiguration<MunicipalityListItem>
    {
        private const string TableName = "MunicipalityList";

        public void Configure(EntityTypeBuilder<MunicipalityListItem> b)
        {
            b.ToTable(TableName, Schema.Legacy)
                .HasKey(x => x.MunicipalityId)
                .ForSqlServerIsClustered(false);

            b.Property(x => x.NisCode);

            b.Property(x => x.PrimaryLanguage);
            b.Property(x => x.SecondaryLanguage);

            b.Property(x => x.DefaultName);
            b.Property(x => x.NameDutch);
            b.Property(x => x.NameFrench);
            b.Property(x => x.NameGerman);
            b.Property(x => x.NameEnglish);

            b.Ignore(x => x.VersionTimestamp);

            b.Property(MunicipalityListItem.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            b.Property(x => x.Status);

            b.HasIndex(x => x.DefaultName).ForSqlServerIsClustered();
            b.HasIndex(x => x.Status);
        }
    }
}
