namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;
    using NodaTime;

    public class TerminationDate : InstantValueObject<TerminationDate>
    {
        public TerminationDate([JsonProperty("value")] Instant date) : base(date) { }
    }
}
