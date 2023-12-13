namespace MunicipalityRegistry.Projections.Integration
{
    using System;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Municipality.Events;
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


        public string? PuriId { get; set; }
        public string? Namespace { get; set; }
        public string? VersionString { get; set; }
        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }

        public long? IdempotenceKey { get; set; }

        public MunicipalityLatestItem() { }
    }

    public sealed class MunicipalityLatestItemConfiguration : IEntityTypeConfiguration<MunicipalityLatestItem>
    {
        public void Configure(EntityTypeBuilder<MunicipalityLatestItem> builder)
        {
            builder
                .ToTable("municipalities2", Schema.Integration)
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

    [ConnectedProjectionName("Integratie gemeente latest item")]
    [ConnectedProjectionDescription("Projectie die de laatste gemeente data voor de integratie database bijhoudt.")]
    public sealed class MunicipalityLatestItemProjections : ConnectedProjection<IntegrationContext>
    {
        public MunicipalityLatestItemProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .MunicipalityLatestItems
                    .AddAsync(
                        new MunicipalityLatestItem()
                        {
                            MunicipalityId = message.Message.MunicipalityId,
                            NisCode = message.Message.NisCode,
                            VersionTimestamp = message.Message.Provenance.Timestamp,
                            IdempotenceKey = message.Position
                        }, ct);
            });
        }
    }
}
