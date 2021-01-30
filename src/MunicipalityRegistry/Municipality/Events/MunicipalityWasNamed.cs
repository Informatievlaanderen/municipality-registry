namespace MunicipalityRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityWasNamed")]
    [EventDescription("De naam van de gemeente (in een bepaalde taal) werd toegevoegd of gewijzigd.")]
    public class MunicipalityWasNamed : IHasProvenance, ISetProvenance
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }

        [EventPropertyDescription("Officiële spelling van de gemeente.")]
        public string Name { get; }

        [EventPropertyDescription("Taal (voluit, EN) waarin de officiële naam staat.")]
        public Language Language { get; }

        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasNamed(
            MunicipalityId municipalityId,
            MunicipalityName municipalityName)
        {
            MunicipalityId = municipalityId;
            Language = municipalityName.Language;
            Name = municipalityName.Name;
        }

        [JsonConstructor]
        private MunicipalityWasNamed(
            Guid municipalityId,
            string name,
            Language language,
            ProvenanceData provenance) :
            this(
                new MunicipalityId(municipalityId),
                new MunicipalityName(name, language)) => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
