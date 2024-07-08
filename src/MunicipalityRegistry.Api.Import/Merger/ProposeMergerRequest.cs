namespace MunicipalityRegistry.Api.Import.Merger
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public sealed class ProposeMergerRequest
    {
        [JsonPropertyName("nisCode")]
        public string NisCode { get; set; }

        [JsonPropertyName("gemeenteVoorstel")]
        public ProposeMergerMunicipalityRequest? ProposeMunicipality { get; set; }

        [JsonPropertyName("fusieVan")]
        public List<string> MergerOf { get; set; }
    }
}
