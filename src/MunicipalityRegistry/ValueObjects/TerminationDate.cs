namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;
    using NodaTime;

    public sealed class TerminationDate : InstantValueObject<TerminationDate>
    {
        public TerminationDate([JsonProperty("value")] Instant date) : base(date) { }
    }
}
