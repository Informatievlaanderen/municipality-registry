namespace MunicipalityRegistry.Api.Import.Merger
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public sealed class ProposeMergerRequest
    {
        [JsonProperty("nisCode")]
        public string NisCode { get; set; }

        [JsonProperty("gemeenteVoorstel")]
        public ProposeMergerMunicipalityRequest? ProposeMunicipality { get; set; }

        [JsonProperty("fusieVan")]
        public List<string> MergerOf { get; set; }
    }
}
