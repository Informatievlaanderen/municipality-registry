namespace MunicipalityRegistry.Api.Import.Merger.Propose
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public sealed class ProposeMergersRequest
    {
        [JsonProperty("fusieJaar")]
        public int MergerYear { get; set; }

        [JsonProperty("gemeenten")]
        public List<ProposeMergerRequest> Municipalities { get; set; }
    }
}
