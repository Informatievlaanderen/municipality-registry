namespace MunicipalityRegistry.Projections.Feed.MunicipalityFeed
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Newtonsoft.Json;
    using NodaTime;
    using JsonSerializer = Newtonsoft.Json.JsonSerializer;

    public sealed class MunicipalityDocument
    {
        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; } = null!;
        public DateTimeOffset LastChangedOnAsDateTimeOffset { get; set; }
        public DateTimeOffset RecordCreatedAtAsDateTimeOffset { get; set; }

        public Instant RecordCreatedAt
        {
            get => Instant.FromDateTimeOffset(RecordCreatedAtAsDateTimeOffset);
            set => RecordCreatedAtAsDateTimeOffset = value.ToBelgianDateTimeOffset();
        }

        public Instant LastChangedOn
        {
            get => Instant.FromDateTimeOffset(LastChangedOnAsDateTimeOffset);
            set => LastChangedOnAsDateTimeOffset = value.ToBelgianDateTimeOffset();
        }

        public MunicipalityJsonDocument Document { get; set; }

        private MunicipalityDocument()
        { }

        public MunicipalityDocument(Guid municipalityId, string nisCode, Instant createdTimestamp) : this()
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            RecordCreatedAt = createdTimestamp;
            LastChangedOn = createdTimestamp;
            Document = new MunicipalityJsonDocument
            {
                MunicipalityId = municipalityId,
                NisCode = nisCode,
                Status = GemeenteStatus.Voorgesteld,
                VersionId = createdTimestamp.ToBelgianDateTimeOffset()
            };
        }
    }

    public sealed record MunicipalityJsonDocument
    {
        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; } = null!;
        public GemeenteStatus Status { get; set; }
        public DateTimeOffset VersionId { get; set; }
        public List<GeografischeNaam> Names { get; set; } = [];
        public List<Taal> OfficialLanguages { get; set; } = [];
        public List<Taal> FacilitiesLanguages { get; set; } = [];
    }

    public sealed class MunicipalityDocumentConfiguration : IEntityTypeConfiguration<MunicipalityDocument>
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private const string TableName = "MunicipalityDocuments";

        public MunicipalityDocumentConfiguration(JsonSerializerSettings serializerSettings)
        {
            _serializerSettings = serializerSettings;
        }

        public void Configure(EntityTypeBuilder<MunicipalityDocument> b)
        {
            b.ToTable(TableName, Schema.Feed)
                .HasKey(x => x.MunicipalityId)
                .IsClustered(false);

            b.Property(x => x.NisCode)
                .HasMaxLength(5);

            b.Property(x => x.Document)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _serializerSettings),
                    v => JsonConvert.DeserializeObject<MunicipalityJsonDocument>(v, _serializerSettings) ?? new MunicipalityJsonDocument());

            b.Property(x => x.RecordCreatedAtAsDateTimeOffset).HasColumnName("RecordCreatedAt");
            b.Property(x => x.LastChangedOnAsDateTimeOffset).HasColumnName("LastChangedOn");

            b.Ignore(x => x.LastChangedOn);
            b.Ignore(x => x.RecordCreatedAt);

            b.HasIndex(x => x.NisCode);
        }
    }
}
