namespace MunicipalityRegistry.Projections.Integration
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MunicipalityRegistry.Infrastructure;
    using NodaTime;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

    public sealed class MunicipalityLatestItem
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; }
        public string? Status { get; set; }

        public bool? OfficialLanguageDutch { get; set; }
        public bool? OfficialLanguageFrench { get; set; }
        public bool? OfficialLanguageGerman { get; set; }
        public bool? OfficialLanguageEnglish { get; set; }

        public bool? FacilityLanguageDutch { get; set; }
        public bool? FacilityLanguageFrench { get; set; }
        public bool? FacilityLanguageGerman { get; set; }
        public bool? FacilityLanguageEnglish { get; set; }

        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }
        public bool IsRemoved { get; set; }

        public string PuriId { get; set; }
        public string Namespace { get; set; }
        public string VersionAsString { get; set; }
        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set
            {
                VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
                VersionAsString = new Rfc3339SerializableDateTimeOffset(value.ToBelgianDateTimeOffset()).ToString();
            }
        }

        public long IdempotenceKey { get; set; }

        public MunicipalityLatestItem()
        { }
    }

    public sealed class MunicipalityLatestItemConfiguration : IEntityTypeConfiguration<MunicipalityLatestItem>
    {
        public void Configure(EntityTypeBuilder<MunicipalityLatestItem> builder)
        {
            builder
                .ToTable("municipality-latest-items", Schema.Integration) // to schema per type
                .HasKey(x => x.MunicipalityId);

            builder.Property(MunicipalityLatestItem.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            builder.Ignore(x => x.VersionTimestamp);

            builder.HasIndex(x => x.NisCode).HasSortOrder(SortOrder.Ascending);
            builder.HasIndex(x => x.NameDutch);
            builder.HasIndex(x => x.NameFrench);
            builder.HasIndex(x => x.NameGerman);
            builder.HasIndex(x => x.NameEnglish);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsRemoved);
        }
    }
}
