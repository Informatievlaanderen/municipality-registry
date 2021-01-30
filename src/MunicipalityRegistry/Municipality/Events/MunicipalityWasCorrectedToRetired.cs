namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;
    using NodaTime;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityWasCorrectedToRetired")]
    [EventDescription("De gemeente kreeg status 'gehistoreerd' (via correctie).")]
    public class MunicipalityWasCorrectedToRetired : IHasProvenance, ISetProvenance
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }

        [EventPropertyDescription("Administratief tijdstip waarop de gemeente status ‘gehistoreerd’ kreeg.")]
        public Instant RetirementDate { get; }

        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasCorrectedToRetired(
            MunicipalityId municipalityId,
            RetirementDate retirementDate)
        {
            MunicipalityId = municipalityId;
            RetirementDate = retirementDate;
        }

        [JsonConstructor]
        private MunicipalityWasCorrectedToRetired(
            Guid municipalityId,
            Instant retirementDate,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                new RetirementDate(retirementDate)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
