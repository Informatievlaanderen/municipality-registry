namespace MunicipalityRegistry
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Newtonsoft.Json;

    public class MunicipalityId : GuidValueObject<MunicipalityId>
    {
        public static MunicipalityId CreateFor(CrabMunicipalityId crabMunicipalityId)
            => new MunicipalityId(crabMunicipalityId.CreateDeterministicId());

        public MunicipalityId([JsonProperty("value")] Guid municipalityId) : base(municipalityId) { }
    }
}
