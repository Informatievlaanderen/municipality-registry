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
        public const string NisCode = "nisCode";
        public const string StatusName = "gemeenteStatus";
        public const string OfficialLanguages = "officieleTalen";
        public const string FacilitiesLanguages = "faciliteitenTalen";
        public const string MunicipalityNames = "gemeentenamen";
    }

    public sealed class MunicipalityCloudTransformEvent
    {
        [JsonProperty("vanObjectId", Order = 0)]
        public required string From { get; set; }

        [JsonProperty("naarObjectId", Order = 1)]
        public required string To { get; set; }

        [JsonProperty("nisCodes", Order = 2)]
        public required List<string> NisCodes { get; set; }
    }

    public sealed class BaseRegistriesCloudEvent
    {
        [JsonProperty("@id", Order = 0)]
        public required string Id { get; set; }

        [JsonProperty("objectId", Order = 1)]
        public required string ObjectId { get; set; }

        [JsonProperty("naamruimte", Order = 2)]
        public required string Namespace { get; set; }

        [JsonProperty("versieId", Order = 3)]
        public required string VersionId { get; set; }

        [JsonProperty("nisCodes", Order = 4)]
        public required List<string> NisCodes { get; set; }

        [JsonProperty("attributen", Order = 5)]
        public required List<BaseRegistriesCloudEventAttribute> Attributes { get; set; } = [];
    }

    public sealed class BaseRegistriesCloudEventAttribute
    {
        public const string BaseRegistriesEventType = "basisregisterseventtype";
        public const string BaseRegistriesCausationId = "basisregisterscausationid";

        [JsonProperty("naam")]
        public string Name { get; set; }

        [JsonProperty("oudeWaarde")]
        public object? OldValue { get; set; }

        [JsonProperty("nieuweWaarde")]
        public object? NewValue { get; set; }

        public BaseRegistriesCloudEventAttribute(string name, object? oldValue, object? newValue)
        {
            Name = name;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
