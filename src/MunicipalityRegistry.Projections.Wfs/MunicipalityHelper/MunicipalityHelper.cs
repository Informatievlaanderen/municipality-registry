namespace MunicipalityRegistry.Projections.Wfs.Municipality
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;

    public class MunicipalityHelper : MunicipalityLanguagesBase
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public Guid? MunicipalityId { get; set; }
        public string? NisCode { get; set; }

        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }

        public MunicipalityStatus? Status { get; set; }

        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }
    }

    public class MunicipalityHelperConfiguration : IEntityTypeConfiguration<MunicipalityHelper>
    {
        private const string TableName = "MunicipalityHelper";

        public void Configure(EntityTypeBuilder<MunicipalityHelper> b)
        {
            b.ToTable(TableName, Schema.Wfs)
                .HasKey(x => x.MunicipalityId)
                .IsClustered(false);

            b.Property(x => x.NisCode);

            b.Property(MunicipalityHelper.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            b.Property(MunicipalityLanguagesBase.OfficialLanguagesBackingPropertyName)
                .HasColumnName("OfficialLanguages");

            b.Property(MunicipalityLanguagesBase.FacilitiesLanguagesBackingPropertyName)
                .HasColumnName("FacilitiesLanguages");

            b.Ignore(x => x.VersionTimestamp);
            b.Ignore(x => x.OfficialLanguages);
            b.Ignore(x => x.FacilitiesLanguages);
            b.Property(x => x.Status);

            b.Property(x => x.NameDutch);
            b.Property(x => x.NameFrench);
            b.Property(x => x.NameGerman);
            b.Property(x => x.NameEnglish);

            b.HasIndex(x => x.NisCode).IsClustered();
        }
    }
}
