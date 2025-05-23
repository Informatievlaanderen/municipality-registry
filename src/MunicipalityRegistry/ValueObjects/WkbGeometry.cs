namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using Newtonsoft.Json;

    public sealed class WkbGeometry : ByteArrayValueObject<WkbGeometry>
    {
        [JsonConstructor]
        public WkbGeometry([JsonProperty("value")] byte[] wkbBytes) : base(wkbBytes) { }

        public WkbGeometry(string wkbBytesHex) : base(wkbBytesHex.ToByteArray()!) { }

        public override string ToString() => Value.ToHexString()!;
    }
}
