namespace MunicipalityRegistry.Api.Import.Merger
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public sealed class ProposeMergersRequest
    {
        [JsonPropertyName("fusieJaar")]
        public int MergerYear { get; set; }

        [JsonPropertyName("gemeenten")]
        public List<ProposeMergerRequest> Municipalities { get; set; }
    }
}
