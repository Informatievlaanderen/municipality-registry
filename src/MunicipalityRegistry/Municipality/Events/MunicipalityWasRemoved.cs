namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityWasRemoved")]
    [EventDescription("De gemeente werd verwijderd uit het register.")]
    public sealed class MunicipalityWasRemoved : IHasProvenance, ISetProvenance, IMessage
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }

        [EventPropertyDescription("NIS-code (= objectidentificator) van de gemeente.")]
        public string NisCode { get; }

        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; } = null!;

        public MunicipalityWasRemoved(MunicipalityId municipalityId, NisCode nisCode)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
        }

        [JsonConstructor]
        private MunicipalityWasRemoved(Guid municipalityId, string nisCode, ProvenanceData provenance) :
            this(new MunicipalityId(municipalityId), new NisCode(nisCode))
            => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
