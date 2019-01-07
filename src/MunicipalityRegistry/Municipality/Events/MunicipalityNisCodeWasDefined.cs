namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityNisCodeWasDefined")]
    [EventDescription("De NIS code van de gemeente werd gedefiniëerd.")]
    public class MunicipalityNisCodeWasDefined : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public string NisCode { get; }
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
