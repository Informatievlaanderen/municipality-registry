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

    public sealed class MunicipalityVersion
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);
        public const string CreatedOnTimestampBackingPropertyName = nameof(CreatedOnTimestampAsDateTimeOffset);

        public long Position { get; set; }

        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; }
        public MunicipalityStatus? Status { get; set; }
        public string? OsloStatus { get; set; }

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
        public string CreatedOnAsString { get; set; }

        private DateTimeOffset CreatedOnTimestampAsDateTimeOffset { get; set; }
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

        public Instant CreatedOnTimestamp
        {
            get => Instant.FromDateTimeOffset(CreatedOnTimestampAsDateTimeOffset);
            set
            {
                CreatedOnTimestampAsDateTimeOffset = value.ToDateTimeOffset();
                CreatedOnAsString = new Rfc3339SerializableDateTimeOffset(value.ToBelgianDateTimeOffset()).ToString();
            }
        }

        public MunicipalityVersion()
        { }

        public MunicipalityVersion CloneAndApplyEventInfo(
            long newPosition,
            Instant lastChangedOn,
            Action<MunicipalityVersion> editFunc)
        {
            var newItem = new MunicipalityVersion
            {
                Position = newPosition,

                MunicipalityId = MunicipalityId,
                NisCode = NisCode,
                Status = Status,
                OsloStatus = OsloStatus,
                CreatedOnTimestamp = CreatedOnTimestamp,

                OfficialLanguageDutch = OfficialLanguageDutch,
                OfficialLanguageFrench = OfficialLanguageFrench,
                OfficialLanguageGerman = OfficialLanguageGerman,
                OfficialLanguageEnglish = OfficialLanguageEnglish,

                FacilityLanguageDutch = FacilityLanguageDutch,
                FacilityLanguageFrench = FacilityLanguageFrench,
                FacilityLanguageGerman = FacilityLanguageGerman,
                FacilityLanguageEnglish = FacilityLanguageEnglish,

                NameDutch = NameDutch,
                NameEnglish = NameEnglish,
                NameFrench = NameFrench,
                NameGerman = NameGerman,

                IsRemoved = IsRemoved,

                PuriId = PuriId,
                Namespace = Namespace,

                VersionTimestamp = lastChangedOn,
            };

            editFunc(newItem);

            return newItem;
        }
    }

    public sealed class MunicipalityLatestEventConfiguration : IEntityTypeConfiguration<MunicipalityVersion>
    {
        public void Configure(EntityTypeBuilder<MunicipalityVersion> builder)
        {
            const string tableName = "municipality_versions";

            builder
                .ToTable(tableName, Schema.Integration) // to schema per type
                .HasKey(x => x.Position);

            builder.Property(x => x.Position).ValueGeneratedNever();

            builder.Property(x => x.Position).HasColumnName("position");
            builder.Property(x => x.MunicipalityId).HasColumnName("municipality_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code")
                .HasMaxLength(5)
                .IsFixedLength();
            builder.Property(x => x.Status).HasColumnName("status");
            builder.Property(x => x.OsloStatus).HasColumnName("oslo_status");

            builder.Property(x => x.OfficialLanguageDutch).HasColumnName("official_language_dutch");
            builder.Property(x => x.OfficialLanguageFrench).HasColumnName("official_language_french");
            builder.Property(x => x.OfficialLanguageGerman).HasColumnName("official_language_german");
            builder.Property(x => x.OfficialLanguageEnglish).HasColumnName("official_language_english");

            builder.Property(x => x.FacilityLanguageDutch).HasColumnName("facility_language_dutch");
            builder.Property(x => x.FacilityLanguageFrench).HasColumnName("facility_language_french");
            builder.Property(x => x.FacilityLanguageGerman).HasColumnName("facility_language_german");
            builder.Property(x => x.FacilityLanguageEnglish).HasColumnName("facility_language_english");

            builder.Property(x => x.NameDutch).HasColumnName("name_dutch");
            builder.Property(x => x.NameFrench).HasColumnName("name_french");
            builder.Property(x => x.NameGerman).HasColumnName("name_german");
            builder.Property(x => x.NameEnglish).HasColumnName("name_english");

            builder.Property(x => x.IsRemoved).HasColumnName("is_removed");

            builder.Property(x => x.PuriId).HasColumnName("puri_id");
            builder.Property(x => x.Namespace).HasColumnName("namespace");
            builder.Property(x => x.VersionAsString).HasColumnName("version_as_string");
            builder.Property(x => x.CreatedOnAsString).HasColumnName("created_on_as_string");

            builder.Ignore(x => x.VersionTimestamp);
            builder.Property(MunicipalityVersion.VersionTimestampBackingPropertyName).HasColumnName("version_timestamp");

            builder.Ignore(x => x.CreatedOnTimestamp);
            builder.Property(MunicipalityVersion.CreatedOnTimestampBackingPropertyName).HasColumnName("created_on_timestamp");

            builder.Property(x => x.MunicipalityId).IsRequired();
            builder.HasIndex(x => x.NisCode).IsDescending(false);
            builder.HasIndex(x => x.IsRemoved);
            builder.HasIndex(MunicipalityVersion.VersionTimestampBackingPropertyName);

            builder.HasIndex(x => x.MunicipalityId);
        }
    }
}
