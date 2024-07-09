namespace MunicipalityRegistry.Api.Import.Merger
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Newtonsoft.Json;

    public sealed class ProposeMergerMunicipalityRequest
    {
        [JsonProperty("officieleTalen")]
        public List<Taal> OfficialLanguages { get; set; } = [];

        [JsonProperty("faciliteitenTalen")]
        public List<Taal> FacilitiesLanguages { get; set; } = [];

        [JsonProperty("namen")]
        public Dictionary<Taal, string> Names { get; set; } = [];
    }
}
