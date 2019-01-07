namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;
    using NodaTime;

    [EventName("MunicipalityWasRetired")]
    [EventDescription("De gemeente werd gehistoriseerd.")]
    public class MunicipalityWasRetired : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public Instant RetirementDate { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasRetired(
            MunicipalityId municipalityId,
            RetirementDate retirementDate)
        {
            MunicipalityId = municipalityId;
            RetirementDate = retirementDate;
        }

        [JsonConstructor]
        private MunicipalityWasRetired(
            Guid municipalityId,
            Instant retirementDate,
            ProvenanceData provenance)
            : this(
                  new MunicipalityId(municipalityId),
                  new RetirementDate(retirementDate)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
