namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalitySecondaryLanguageWasCorrected")]
    [EventDescription("De secundaire taal van de gemeente werd gecorrigeerd.")]
    public class MunicipalitySecondaryLanguageWasCorrected : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public Language Language { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalitySecondaryLanguageWasCorrected(
            MunicipalityId municipalityId,
            Language language)
        {
            MunicipalityId = municipalityId;
            Language = language;
        }

        [JsonConstructor]
        private MunicipalitySecondaryLanguageWasCorrected(
            Guid municipalityId,
            Language language,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                language) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
