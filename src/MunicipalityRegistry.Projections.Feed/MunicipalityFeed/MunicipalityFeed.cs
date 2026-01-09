namespace MunicipalityRegistry.Projections.Feed.MunicipalityFeed
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using CloudNative.CloudEvents;
    using CloudNative.CloudEvents.Http;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Newtonsoft.Json;

    public class MunicipalityFeedItem
    {
        public long Id { get; set; }
        public int Page { get; set; }
        public long Position { get; set; }

        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; } = null!;

        public Application? Application { get; set; }
        public Modification? Modification { get; set; }
        public string? Operator { get; set; }
        public Organisation? Organisation { get; set; }
        public string? Reason { get; set; }
        public string CloudEventAsString { get; set; } = null!;

        private MunicipalityFeedItem() { }

        public MunicipalityFeedItem(long position, int page, Guid municipalityId, string nisCode) : this()
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            Page = page;
            Position = position;
        }
    }

    public class MunicipalityFeedConfiguration : IEntityTypeConfiguration<MunicipalityFeedItem>
    {
        private const string TableName = "MunicipalityFeed";

        public void Configure(EntityTypeBuilder<MunicipalityFeedItem> b)
        {
            b.ToTable(TableName, Schema.Feed)
                .HasKey(x => x.Id)
                .IsClustered();

            b.Property(x => x.Id)
                .UseHiLo("MunicipalityFeedSequence", Schema.Feed);

            b.Property(x => x.CloudEventAsString)
                .HasColumnName("CloudEvent")
                .IsRequired();

            b.Property(x => x.MunicipalityId).IsRequired();
            b.Property(x => x.NisCode).HasMaxLength(5).IsRequired();

            b.Property(x => x.Application);
            b.Property(x => x.Modification);
            b.Property(x => x.Operator);
            b.Property(x => x.Organisation);
            b.Property(x => x.Reason);

            b.HasIndex(x => x.Position);
            b.HasIndex(x => x.Page);
            b.HasIndex(x => x.MunicipalityId);
            b.HasIndex(x => x.NisCode);
        }
    }
}
