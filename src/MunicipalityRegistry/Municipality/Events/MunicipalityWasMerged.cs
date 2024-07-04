namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventName("MunicipalityWasMerged")]
    [EventDescription("De gemeente werd gefusioneerd.")]
    public sealed class MunicipalityWasMerged : IHasProvenance, ISetProvenance, IMessage
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }

        [EventPropertyDescription("NIS-code (= objectidentificator) van de gemeente.")]
        public string NisCode { get; }

        [EventPropertyDescription("Interne GUIDs van de gemeenten waarmee gefusioneerd werd.")]
        public List<Guid> MunicipalityIdsToMergeWith { get; }

        [EventPropertyDescription("NIS-codes (= objectidentificatoren) van de gemeenten waarmee gefusioneerd werd.")]
        public List<string> NisCodesToMergeWith { get; }

        [EventPropertyDescription("Interne GUID van de nieuwe gemeente.")]
        public Guid NewMunicipalityId { get; }

        [EventPropertyDescription("NIS-code (= objectidentificator) van de nieuwe gemeente.")]
        public string NewNisCode { get; }

        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasMerged(
            MunicipalityId municipalityId,
            NisCode nisCode,
            IEnumerable<MunicipalityId> municipalityIdsToMergeWith,
            IEnumerable<NisCode> nisCodesToMergeWith,
            MunicipalityId newMunicipalityId,
            NisCode newNisCode)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            MunicipalityIdsToMergeWith = municipalityIdsToMergeWith.Select(x => (Guid)x).ToList();
            NisCodesToMergeWith = nisCodesToMergeWith.Select(x => (string)x).ToList();
            NewMunicipalityId = newMunicipalityId;
            NewNisCode = newNisCode;
        }

        [JsonConstructor]
        private MunicipalityWasMerged(
            Guid municipalityId,
            string nisCode,
            List<Guid> municipalityIdsToMergeWith,
            List<string> nisCodesToMergeWith,
            Guid newMunicipalityId,
            string newNisCode,
            ProvenanceData provenance) : this(
            new MunicipalityId(municipalityId),
            new NisCode(nisCode),
            municipalityIdsToMergeWith.Select(x => new MunicipalityId(x)).ToList(),
            nisCodesToMergeWith.Select(x => new NisCode(x)).ToList(),
            new MunicipalityId(newMunicipalityId),
            new NisCode(newNisCode)) =>
            ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);

    }
}
