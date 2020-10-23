namespace MunicipalityRegistry.Projections.Extract.MunicipalityExtract
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Newtonsoft.Json;

    public class MunicipalityExtractItem
    {
        public const string OfficialLanguagesBackingPropertyName = nameof(OfficialLanguagesAsString);

        public Guid? MunicipalityId { get; set; }
        public string? NisCode { get; set; }
        public byte[]? DbaseRecord { get; set; }
        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameEnglish { get; set; }
        public string? NameGerman { get; set; }

        public IReadOnlyCollection<Language> OfficialLanguages
        {
            get => GetDeserializedOfficialLanguages();
            set => OfficialLanguagesAsString = JsonConvert.SerializeObject(value);
        }

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
    }

    public class MunicipalityExtractItemConfiguration : IEntityTypeConfiguration<MunicipalityExtractItem>
    {
        private const string TableName = "Municipality";

        public void Configure(EntityTypeBuilder<MunicipalityExtractItem> builder)
        {
            builder.ToTable(TableName, Schema.Extract)
                .HasKey(p => p.MunicipalityId)
                .IsClustered(false);

            builder.Property(p => p.NisCode);
            builder.Property(p => p.DbaseRecord);
            builder.Property(p => p.NameDutch);
            builder.Property(p => p.NameEnglish);
            builder.Property(p => p.NameFrench);
            builder.Property(p => p.NameGerman);
            builder.Property(MunicipalityExtractItem.OfficialLanguagesBackingPropertyName)
                .HasColumnName("OfficialLanguages");

            builder.Ignore(p => p.OfficialLanguages);

            builder.HasIndex(p => p.NisCode).IsClustered();
        }
    }
}
