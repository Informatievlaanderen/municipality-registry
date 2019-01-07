namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityWasCorrectedToCurrent")]
    [EventDescription("De gemeente werd in gebruik genomen via correctie.")]
    public class MunicipalityWasCorrectedToCurrent : IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasCorrectedToCurrent(
            MunicipalityId municipalityId)
        {
            MunicipalityId = municipalityId;
        }

        [JsonConstructor]
        private MunicipalityWasCorrectedToCurrent(
            Guid municipalityId,
            ProvenanceData provenance)
            : this(
                new MunicipalityId(municipalityId)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
