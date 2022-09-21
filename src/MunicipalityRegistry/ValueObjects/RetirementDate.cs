namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;
    using NodaTime;

    public sealed class RetirementDate : InstantValueObject<RetirementDate>
    {
        public RetirementDate([JsonProperty("value")] Instant date) : base(date) { }
    }
}
