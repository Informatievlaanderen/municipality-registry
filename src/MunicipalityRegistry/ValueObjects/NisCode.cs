namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public sealed class NisCode : StringValueObject<NisCode>
    {
        public NisCode([JsonProperty("value")] string nisCode) : base(nisCode)
        {
            if (string.IsNullOrWhiteSpace(nisCode))
                throw new NoNisCodeException("NisCode of a municipality cannot be empty.");
        }
    }
}
