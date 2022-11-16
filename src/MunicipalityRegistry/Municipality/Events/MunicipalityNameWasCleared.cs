namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityNameWasCleared")]
    [EventDescription("De naam van de gemeente (in een bepaalde taal) werd gewist.")]
    public sealed class MunicipalityNameWasCleared : IHasProvenance, ISetProvenance, IMessage
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }

        [EventPropertyDescription("Taal waarvoor de officiële naam gewist werd. Mogelijkheden: Dutch, French of German.")]
        public Language Language { get; }

        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityNameWasCleared(
            MunicipalityId municipalityId,
            Language language)
        {
            MunicipalityId = municipalityId;
            Language = language;
        }

        [JsonConstructor]
        private MunicipalityNameWasCleared(
            Guid municipalityId,
            Language language,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                language) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
