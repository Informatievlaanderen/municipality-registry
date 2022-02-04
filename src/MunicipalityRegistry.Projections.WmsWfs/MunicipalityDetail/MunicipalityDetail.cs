namespace MunicipalityRegistry.Projections.WmsWfs.MunicipalityDetail
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;

    public class MunicipalityDetail
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public Guid MunicipalityId { get; set; }
        public string? NisCode { get; set; }
        public string? Id { get; set; }
        public string? NameDutch { get; set; }
        public string? Status { get; set; }
        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }
        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }
    }

    public class MunicipalityDetailConfiguration : IEntityTypeConfiguration<MunicipalityDetail>
    {
        private const string TableName = "MunicipalityDetails";

        public void Configure(EntityTypeBuilder<MunicipalityDetail> b)
        {
            b.ToTable(TableName, Schema.WmsWfs)
                .HasKey(x => x.MunicipalityId)
                .IsClustered(false);

            b.Property(x => x.NisCode);

            b.Property(MunicipalityDetail.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            b.Ignore(x => x.VersionTimestamp);
            b.Property(x => x.Status);
            b.Property(x => x.NameDutch);
            b.Property(x => x.Id);
            b.HasIndex(x => x.NisCode).IsClustered();
        }
    }
}
