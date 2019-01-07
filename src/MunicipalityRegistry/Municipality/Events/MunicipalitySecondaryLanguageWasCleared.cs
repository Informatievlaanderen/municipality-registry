namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalitySecondaryLanguageWasCleared")]
    [EventDescription("De secundaire taal van de gemeente werd verwijderd.")]
    public class MunicipalitySecondaryLanguageWasCleared : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalitySecondaryLanguageWasCleared(
            MunicipalityId municipalityId)
        {
            MunicipalityId = municipalityId;
        }

        [JsonConstructor]
        private MunicipalitySecondaryLanguageWasCleared(
            Guid municipalityId,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
