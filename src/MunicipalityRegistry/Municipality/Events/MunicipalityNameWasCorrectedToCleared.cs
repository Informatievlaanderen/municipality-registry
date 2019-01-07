namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityNameWasCorrectedToCleared")]
    [EventDescription("De naam van de gemeente werd verwijderd voor een bepaalde taal via correctie.")]
    public class MunicipalityNameWasCorrectedToCleared : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public Language Language { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityNameWasCorrectedToCleared(
            MunicipalityId municipalityId,
            Language language)
        {
            MunicipalityId = municipalityId;
            Language = language;
        }

        [JsonConstructor]
        private MunicipalityNameWasCorrectedToCleared(
            Guid municipalityId,
            Language language,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                language) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
