namespace MunicipalityRegistry.Projections.Legacy.MunicipalityList
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Newtonsoft.Json;
    using NodaTime;

    public class MunicipalityListItem
    {
        public const string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);
        public const string OfficialLanguagesBackingPropertyName = nameof(OfficialLanguagesAsString);

        public Guid? MunicipalityId { get; set; }
        public string? NisCode { get; set; }

        public string? DefaultName { get; set; }
        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }


        public string? NameDutchSearch { get; set; }
        public string? NameFrenchSearch { get; set; }
        public string? NameGermanSearch { get; set; }
        public string? NameEnglishSearch { get; set; }

        public MunicipalityStatus? Status { get; set; }

        public IReadOnlyCollection<Language> OfficialLanguages
        {
            get => GetDeserializedOfficialLanguages();
            set => OfficialLanguagesAsString = JsonConvert.SerializeObject(value);
        }

        private string? OfficialLanguagesAsString { get; set; }

        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }

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

        public bool IsRemoved { get; set; }
    }

    public class MunicipalityListConfiguration : IEntityTypeConfiguration<MunicipalityListItem>
    {
        private const string TableName = "MunicipalityList";

        public void Configure(EntityTypeBuilder<MunicipalityListItem> b)
        {
            b.ToTable(TableName, Schema.Legacy)
                .HasKey(x => x.MunicipalityId)
                .IsClustered(false);

            b.Property(x => x.NisCode);

            b.Property(x => x.DefaultName);
            b.Property(x => x.NameDutch);
            b.Property(x => x.NameFrench);
            b.Property(x => x.NameGerman);
            b.Property(x => x.NameEnglish);

            b.Property(x => x.NameDutchSearch);
            b.Property(x => x.NameFrenchSearch);
            b.Property(x => x.NameGermanSearch);
            b.Property(x => x.NameEnglishSearch);

            b.Property(x => x.IsRemoved)
                .HasDefaultValue(false);

            b.Ignore(x => x.VersionTimestamp);
            b.Ignore(x => x.OfficialLanguages);

            b.Property(MunicipalityListItem.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            b.Property(MunicipalityListItem.OfficialLanguagesBackingPropertyName)
                .HasColumnName("OfficialLanguages");

            b.Property(x => x.Status);

            b.HasIndex(x => x.DefaultName);
            b.HasIndex(x => x.NameDutchSearch);
            b.HasIndex(x => x.NameFrenchSearch);
            b.HasIndex(x => x.NameGermanSearch);
            b.HasIndex(x => x.NameEnglishSearch);
            b.HasIndex(x => x.NisCode).IsClustered();
            b.HasIndex(x => x.IsRemoved);
        }
    }
}
