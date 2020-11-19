namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityNisCodeWasDefined")]
    [EventDescription("De persistente lokale identificator (NIS-code) van de gemeente werd bepaald.")]
    public class MunicipalityNisCodeWasDefined : IHasProvenance, ISetProvenance
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }
        
        [EventPropertyDescription("NIS-code (= objectidentificator) van de gemeente.")]
        public string NisCode { get; }
        
        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityNisCodeWasDefined(
            MunicipalityId municipalityId,
            NisCode nisCode)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
        }

        [JsonConstructor]
        private MunicipalityNisCodeWasDefined(
            Guid municipalityId,
            string nisCode,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                new NisCode(nisCode)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
