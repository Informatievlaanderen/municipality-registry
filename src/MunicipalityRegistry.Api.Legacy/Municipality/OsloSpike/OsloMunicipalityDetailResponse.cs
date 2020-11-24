namespace MunicipalityRegistry.Api.Legacy.Municipality.Responses
{
    using Newtonsoft.Json;

    public class OsloMunicipalityDetailResponse
    {
        public OsloMunicipalityDetailResponse(
            string naamruimte,
            string id)
        {
            Id = naamruimte + "/" + id;
            Type = "https://data.vlaanderen.be/ns/generiek#Gemeente";
            Status = "https://data.vlaanderen.be/id/concept/gemeentestatus/ingebruik";
        }

        [JsonProperty(Required = Required.DisallowNull, PropertyName = "@id", Order = 1)]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "@type")]
        public string Type { get; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; private set; }
    }
}
