namespace MunicipalityRegistry.Producer.Ldes
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class MunicipalityLdes
    {
        private static readonly JObject Context = JObject.Parse(@"
{
    ""@version"": 1.1,
    ""@base"": ""https://basisregisters.vlaanderen.be/implementatiemodel/adressenregister"",
    ""@vocab"": ""#"",
    ""identificator"": ""@nest"",
    ""id"": ""@id"",
    ""versieId"": {
      ""@id"": ""https://data.vlaanderen.be/ns/generiek#versieIdentificator"",
      ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
    },
    ""naamruimte"": {
      ""@id"": ""https://data.vlaanderen.be/ns/generiek#naamruimte"",
      ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
    },
    ""objectId"": {
      ""@id"": ""https://data.vlaanderen.be/ns/generiek#lokaleIdentificator"",
      ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
    },
    ""gemeentenamen"": {
      ""@id"": ""https://data.vlaanderen.be/ns/adres#Gemeentenaam"",
      ""@container"": ""@language""
    },
    ""gemeenteStatus"": {
      ""@id"": ""https://basisregisters.vlaanderen.be/implementatiemodel/adressenregister#Gemeente%3Astatus"",
      ""@type"": ""@id"",
      ""@context"": {
        ""@base"": ""https://data.vlaanderen.be/doc/concept/gemeentestatus/""
      }
    },
    ""officieleTalen"": {
      ""@id"": ""https://basisregisters.vlaanderen.be/implementatiemodel/adressenregister#Gemeente%3Aoffici%C3%ABle%20taal""
    },
    ""faciliteitenTalen"": {
      ""@id"": ""https://basisregisters.vlaanderen.be/implementatiemodel/adressenregister#Gemeente%3Afaciliteitentaal""
    }
}");

        [JsonProperty("@context", Order = 0)]
        public JObject LdContext => Context;

        [JsonProperty("@type", Order = 1)]
        public string Type => "Gemeente";

        [JsonProperty("Identificator", Order = 2)]
        public GemeenteIdentificator Identificator { get; private set; }

        [JsonProperty("OfficieleTalen", Order = 3)]
        public List<Taal> OfficialLanguages { get; private set; }

        [JsonProperty("FaciliteitenTalen", Order = 4)]
        public List<Taal> FacilitiesLanguages { get; private set; }

        [JsonProperty("Gemeentenamen", Order = 5)]
        public Dictionary<string, string> MunicipalityNames { get; private set; }

        [JsonProperty("GemeenteStatus", Order = 6)]
        public GemeenteStatus Status { get; private set; }

        [JsonProperty("isVerwijderd", Order = 7)]
        public bool IsRemoved => false;

        public MunicipalityLdes(MunicipalityDetail municipality, string osloNamespace)
        {
            Identificator = new GemeenteIdentificator(
                osloNamespace,
                municipality.NisCode,
                municipality.VersionTimestamp.ToBelgianDateTimeOffset()
            );
            OfficialLanguages = municipality.OfficialLanguages.Select(x => x.ConvertFromLanguage()).ToList();
            FacilitiesLanguages = municipality.FacilitiesLanguages.Select(x => x.ConvertFromLanguage()).ToList();
            Status = municipality.Status.ConvertFromMunicipalityStatus();

            MunicipalityNames = new Dictionary<string, string>(
                new[]
                    {
                        ("nl", municipality.NameDutch),
                        ("fr", municipality.NameFrench),
                        ("de", municipality.NameGerman),
                        ("en", municipality.NameEnglish)
                    }
                    .Where(pair => !string.IsNullOrEmpty(pair.Item2))
                    .ToDictionary(pair => pair.Item1, pair => pair.Item2)!
            );
        }
    }
}
