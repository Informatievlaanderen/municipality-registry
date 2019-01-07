namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityPrimaryLanguageWasCorrectedToCleared")]
    [EventDescription("De primaire taal van de gemeente werd verwijderd via correctie.")]
    public class MunicipalityPrimaryLanguageWasCorrectedToCleared : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityPrimaryLanguageWasCorrectedToCleared(
            MunicipalityId municipalityId)
        {
            MunicipalityId = municipalityId;
        }

        [JsonConstructor]
        private MunicipalityPrimaryLanguageWasCorrectedToCleared(
            Guid municipalityId,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
