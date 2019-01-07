namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;
    using NodaTime;

    public class RetirementDate : InstantValueObject<RetirementDate>
    {
        public RetirementDate([JsonProperty("value")] Instant date) : base(date) { }
    }
}
