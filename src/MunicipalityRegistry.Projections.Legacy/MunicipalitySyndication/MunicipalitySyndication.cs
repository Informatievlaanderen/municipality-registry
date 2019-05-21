namespace MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;

    public class MunicipalitySyndicationItem : MunicipalityLanguagesBase
    {
        public long Position { get; set; }

        public Guid? MunicipalityId { get; set; }
        public string NisCode { get; set; }
        public string ChangeType { get; set; }

        public string DefaultName { get; set; }
        public string NameDutch { get; set; }
        public string NameFrench { get; set; }
        public string NameGerman { get; set; }
        public string NameEnglish { get; set; }

        public MunicipalityStatus? Status { get; set; }

        public DateTimeOffset RecordCreatedAtAsDateTimeOffset { get; set; }
        public DateTimeOffset LastChangedOnAsDateTimeOffset { get; set; }

        public Instant RecordCreatedAt
        {
            get => Instant.FromDateTimeOffset(RecordCreatedAtAsDateTimeOffset);
            set => RecordCreatedAtAsDateTimeOffset = value.ToDateTimeOffset();
        }

        public Instant LastChangedOn
        {
            get => Instant.FromDateTimeOffset(LastChangedOnAsDateTimeOffset);
            set => LastChangedOnAsDateTimeOffset = value.ToDateTimeOffset();
        }

        public Application? Application { get; set; }
        public Modification? Modification { get; set; }
        public string Operator { get; set; }
        public Organisation? Organisation { get; set; }
        public Plan? Plan { get; set; }
        public string EventDataAsXml { get; set; }

        public MunicipalitySyndicationItem CloneAndApplyEventInfo(
            long newPosition,
            string eventName,
            Instant lastChangedOn,
            Action<MunicipalitySyndicationItem> editFunc)
        {
            var newItem = new MunicipalitySyndicationItem
            {
                Position = newPosition,

                MunicipalityId = MunicipalityId,
                NisCode = NisCode,
                ChangeType = eventName,

                OfficialLanguages = OfficialLanguages,
                FacilitiesLanguages = FacilitiesLanguages,

                DefaultName = DefaultName,
                NameDutch = NameDutch,
                NameEnglish = NameEnglish,
                NameFrench = NameFrench,
                NameGerman = NameGerman,

                Status = Status,

                RecordCreatedAt = RecordCreatedAt,
                LastChangedOn = lastChangedOn,

                Application = Application,
                Modification = Modification,
                Operator = Operator,
                Organisation = Organisation,
                Plan = Plan
            };

            editFunc(newItem);

            return newItem;
        }
    }

    public class MunicipalitySyndicationConfiguration : IEntityTypeConfiguration<MunicipalitySyndicationItem>
    {
        private const string TableName = "MunicipalitySyndication";

        public void Configure(EntityTypeBuilder<MunicipalitySyndicationItem> b)
        {
            b.ToTable(TableName, Schema.Legacy)
                .HasKey(x => x.Position)
                .ForSqlServerIsClustered();

            b.Property(x => x.Position).ValueGeneratedNever();

            b.Property(x => x.MunicipalityId).IsRequired();
            b.Property(x => x.NisCode);
            b.Property(x => x.ChangeType);

            b.Property(x => x.DefaultName);
            b.Property(x => x.NameDutch);
            b.Property(x => x.NameFrench);
            b.Property(x => x.NameGerman);
            b.Property(x => x.NameEnglish);
            b.Property(x => x.Status);

            b.Property(MunicipalityLanguagesBase.OfficialLanguagesBackingPropertyName)
                .HasColumnName("OfficialLanguages");

            b.Property(MunicipalityLanguagesBase.FacilitiesLanguagesBackingPropertyName)
                .HasColumnName("FacilitiesLanguages");

            b.Property(x => x.RecordCreatedAtAsDateTimeOffset).HasColumnName("RecordCreatedAt");
            b.Property(x => x.LastChangedOnAsDateTimeOffset).HasColumnName("LastChangedOn");

            b.Property(x => x.Application);
            b.Property(x => x.Modification);
            b.Property(x => x.Operator);
            b.Property(x => x.Organisation);
            b.Property(x => x.Plan);
            b.Property(x => x.EventDataAsXml);

            b.Ignore(x => x.RecordCreatedAt);
            b.Ignore(x => x.LastChangedOn);
            b.Ignore(x => x.OfficialLanguages);
            b.Ignore(x => x.FacilitiesLanguages);

            b.HasIndex(x => x.MunicipalityId);
        }
    }
}
