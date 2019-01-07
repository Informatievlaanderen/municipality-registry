namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityNisCodeWasCorrected")]
    [EventDescription("De NIS code van de gemeente werd gecorrigeerd.")]
    public class MunicipalityNisCodeWasCorrected : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public string NisCode { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityNisCodeWasCorrected(
            MunicipalityId municipalityId,
            NisCode nisCode)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
        }

        [JsonConstructor]
        private MunicipalityNisCodeWasCorrected(
            Guid municipalityId,
            string nisCode,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                new NisCode(nisCode)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
