﻿namespace MunicipalityRegistry.Projections.Integration
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
        public bool IsFlemishRegion { get; private set; }
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

        public MunicipalityLatestItem()
        { }
    }

    public sealed class MunicipalityLatestItemConfiguration : IEntityTypeConfiguration<MunicipalityLatestItem>
    {
        public void Configure(EntityTypeBuilder<MunicipalityLatestItem> builder)
        {
            builder
                .ToTable("municipality_latest_items", Schema.Integration) // to schema per type
                .HasKey(x => x.MunicipalityId);

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
            builder.Property(MunicipalityLatestItem.VersionTimestampBackingPropertyName).HasColumnName("version_timestamp");

            builder.Ignore(x => x.VersionTimestamp);

            builder.Property(x => x.IsFlemishRegion)
                .HasColumnName("is_flemish_region")
                .HasComputedColumnSql(
                    "nis_code LIKE '1%' OR nis_code LIKE '3%' OR nis_code LIKE '4%' OR nis_code LIKE '7%' OR nis_code LIKE '23%' OR nis_code LIKE '24%'", stored:true);

            builder.HasIndex(x => x.NisCode).IsDescending(false);
            builder.HasIndex(x => x.NameDutch);
            builder.HasIndex(x => x.NameFrench);
            builder.HasIndex(x => x.NameGerman);
            builder.HasIndex(x => x.NameEnglish);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.OsloStatus);
            builder.HasIndex(x => x.IsRemoved);
        }
    }
}
