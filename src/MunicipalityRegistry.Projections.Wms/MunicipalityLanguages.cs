namespace MunicipalityRegistry.Projections.Wms
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MunicipalityLanguagesBase
    {
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
}
