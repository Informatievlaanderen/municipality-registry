namespace MunicipalityRegistry.Projections.Feed.Contract
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public static class MunicipalityEventTypes
    {
        public const string CreateV1 = "basisregisters.municipality.create.v1";
        public const string UpdateV1 = "basisregisters.municipality.update.v1";
        public const string DeleteV1 = "basisregisters.municipality.delete.v1";
        public const string TransformV1 = "basisregisters.municipality.transform.v1";
    }

    public static class MunicipalityAttributeNames
    {
        public const string StatusName = "status";
        public const string OfficialLanguages = "officieleTaal";
        public const string FacilitiesLanguages = "faciliteitenTaal";
        public const string MunicipalityNames = "gemeentenaam";
    }

    public sealed class MunicipalityCloudTransformEvent
    {
        [JsonProperty("vanId", Order = 0)]
        public required string From { get; set; }

        [JsonProperty("naarId", Order = 1)]
        public required string To { get; set; }

        [JsonProperty("nisCodes", Order = 2)]
        public required List<string> NisCodes { get; set; }
    }
}
