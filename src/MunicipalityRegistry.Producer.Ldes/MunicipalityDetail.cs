namespace MunicipalityRegistry.Producer.Ldes
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MunicipalityRegistry.Infrastructure;
    using Newtonsoft.Json;
    using NodaTime;

    public sealed class MunicipalityDetail
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public Guid? MunicipalityId { get; set; }
        public string? NisCode { get; set; }

        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }

        public bool IsRemoved { get; set; }
        public MunicipalityStatus? Status { get; set; }

        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }

        public const string OfficialLanguagesBackingPropertyName = nameof(OfficialLanguagesAsString);
        public const string FacilitiesLanguagesBackingPropertyName = nameof(FacilitiesLanguagesAsString);

        public IReadOnlyCollection<Language> OfficialLanguages
        {
            get => GetDeserializedOfficialLanguages();
            set => OfficialLanguagesAsString = JsonConvert.SerializeObject(value);
        }

        public IReadOnlyCollection<Language> FacilitiesLanguages
        {
            get => GetDeserializedFacilitiesLanguages();
            set => FacilitiesLanguagesAsString = JsonConvert.SerializeObject(value);
        }

        private string? FacilitiesLanguagesAsString { get; set; }
        private string? OfficialLanguagesAsString { get; set; }

        public void AddOfficialLanguage(Language language)
        {
            var languages = GetDeserializedOfficialLanguages();
            languages.Add(language);
            OfficialLanguages = languages;
        }

        public void RemoveOfficialLanguage(Language language)
        {
            var languages = GetDeserializedOfficialLanguages();
            languages.Remove(language);
            OfficialLanguages = languages;
        }

        private List<Language> GetDeserializedOfficialLanguages()
        {
            return string.IsNullOrEmpty(OfficialLanguagesAsString)
                ? new List<Language>()
                : JsonConvert.DeserializeObject<List<Language>>(OfficialLanguagesAsString);
        }

        public void AddFacilitiesLanguage(Language language)
        {
            var languages = GetDeserializedFacilitiesLanguages();
            languages.Add(language);
            FacilitiesLanguages = languages;
        }

        public void RemoveFacilitiesLanguage(Language language)
        {
            var languages = GetDeserializedFacilitiesLanguages();
            languages.Remove(language);
            FacilitiesLanguages = languages;
        }

        private List<Language> GetDeserializedFacilitiesLanguages()
        {
            return string.IsNullOrEmpty(FacilitiesLanguagesAsString)
                ? new List<Language>()
                : JsonConvert.DeserializeObject<List<Language>>(FacilitiesLanguagesAsString);
        }
    }

    public class MunicipalityLdesConfiguration : IEntityTypeConfiguration<MunicipalityDetail>
    {
        private const string TableName = "Municipalities";

        public void Configure(EntityTypeBuilder<MunicipalityDetail> b)
        {
            b.ToTable(TableName, Schema.ProducerLdes)
                .HasKey(x => x.MunicipalityId)
                .IsClustered(false);

            b.Property(x => x.NisCode);

            b.Property(MunicipalityDetail.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            b.Property(MunicipalityDetail.OfficialLanguagesBackingPropertyName)
                .HasColumnName("OfficialLanguages");

            b.Property(MunicipalityDetail.FacilitiesLanguagesBackingPropertyName)
                .HasColumnName("FacilitiesLanguages");

            b.Ignore(x => x.VersionTimestamp);
            b.Ignore(x => x.OfficialLanguages);
            b.Ignore(x => x.FacilitiesLanguages);
            b.Property(x => x.Status);

            b.Property(x => x.NameDutch);
            b.Property(x => x.NameFrench);
            b.Property(x => x.NameGerman);
            b.Property(x => x.NameEnglish);
            b.Property(x => x.IsRemoved)
                .HasDefaultValue(false);

            b.HasIndex(x => x.NisCode).IsClustered();
        }
    }
}
