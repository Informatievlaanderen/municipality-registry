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
        public const string StatusName = "gemeenteStatus";
        public const string OfficialLanguages = "officieleTalen";
        public const string FacilitiesLanguages = "faciliteitenTalen";
        public const string MunicipalityNames = "gemeentenamen";
    }

    public sealed class MunicipalityCloudTransformEvent
    {
        [JsonProperty("nisCodes", Order = 0)]
        public required List<string> NisCodes { get; set; }

        [JsonProperty("transformatie", Order = 1)]
        public required MunicipalityCloudTransformData Transformatie { get; set; }
    }

    public sealed class MunicipalityCloudTransformData
    {
        [JsonProperty("vanIds", Order = 0)]
        public required List<string> FromIds { get; set; }

        [JsonProperty("naarId", Order = 1)]
        public required string To { get; set; }
    }
}
