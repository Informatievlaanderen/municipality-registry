namespace MunicipalityRegistry.Api.Import.Merger
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;

    public sealed class ProposeMergerMunicipalityRequest
    {
        [JsonPropertyName("officieleTalen")]
        public List<Taal> OfficialLanguages { get; set; } = [];

        [JsonPropertyName("faciliteitenTalen")]
        public List<Taal> FacilitiesLanguages { get; set; } = [];

        [JsonPropertyName("namen")]
        public Dictionary<Taal, string> Names { get; set; } = [];
    }
}
