namespace MunicipalityRegistry.Projections.Legacy.MunicipalityVersion
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;

    public class MunicipalityVersion : MunicipalityLanguagesBase
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

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
        public string Reason { get; set; }

        public MunicipalityVersion CloneAndApplyEventInfo(
            long newPosition,
            Action<MunicipalityVersion> editFunc)
        {
            var newItem = new MunicipalityVersion
            {
                Position = newPosition,

                MunicipalityId = MunicipalityId,
                NisCode = NisCode,

                NameDutch = NameDutch,
                NameEnglish = NameEnglish,
                NameFrench = NameFrench,
                NameGerman = NameGerman,

                Status = Status,

                OfficialLanguages = OfficialLanguages,
                FacilitiesLanguages = FacilitiesLanguages,

                VersionTimestamp = VersionTimestamp,

                Application = Application,
                Modification = Modification,
                Operator = Operator,
                Organisation = Organisation,
                Reason = Reason
            };

            editFunc(newItem);

            return newItem;
        }
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

            builder.Property(MunicipalityLanguagesBase.OfficialLanguagesBackingPropertyName)
                .HasColumnName("OfficialLanguages");

            builder.Property(MunicipalityLanguagesBase.FacilitiesLanguagesBackingPropertyName)
                .HasColumnName("FacilitiesLanguages");

            builder.Property(x => x.Application);
            builder.Property(x => x.Modification);
            builder.Property(x => x.Operator);
            builder.Property(x => x.Organisation);
            builder.Property(x => x.Reason);

            builder.Ignore(x => x.VersionTimestamp);
            builder.Ignore(x => x.OfficialLanguages);
            builder.Ignore(x => x.FacilitiesLanguages);

            builder.HasIndex(x => x.NisCode).ForSqlServerIsClustered();
            builder.HasIndex(x => x.MunicipalityId);
            builder.HasIndex(x => x.Position);
        }
    }
}
