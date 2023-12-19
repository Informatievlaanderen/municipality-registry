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

        public long Position { get; set; }

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

                VersionTimestamp = lastChangedOn
            };

            editFunc(newItem);

            return newItem;
        }
    }

    public sealed class MunicipalityLatestEventConfiguration : IEntityTypeConfiguration<MunicipalityVersion>
    {
        public void Configure(EntityTypeBuilder<MunicipalityVersion> builder)
        {
            const string tableName = "municipality-versions";

            builder
                .ToTable(tableName, Schema.Integration) // to schema per type
                .HasKey(x => x.Position);

            builder.Property(x => x.Position).ValueGeneratedNever();

            builder.Property(MunicipalityVersion.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            builder.Ignore(x => x.VersionTimestamp);

            builder.Property(x => x.MunicipalityId).IsRequired();
            builder.HasIndex(x => x.NisCode).HasSortOrder(SortOrder.Ascending);
            builder.HasIndex(x => x.NameDutch);
            builder.HasIndex(x => x.NameFrench);
            builder.HasIndex(x => x.NameGerman);
            builder.HasIndex(x => x.NameEnglish);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsRemoved);

            builder.HasIndex(x => x.MunicipalityId);
        }
    }
}
