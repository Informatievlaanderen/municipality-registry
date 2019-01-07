namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityWasRegistered")]
    [EventDescription("De gemeente werd aangemaakt.")]
    public class MunicipalityWasRegistered : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public string NisCode { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasRegistered(
            MunicipalityId municipalityId,
            NisCode nisCode)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
        }

        [JsonConstructor]
        private MunicipalityWasRegistered(
            Guid municipalityId,
            string nisCode,
            ProvenanceData provenance)
            : this(
                new MunicipalityId(municipalityId),
                new NisCode(nisCode)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
