namespace MunicipalityRegistry.Municipality.Events
{
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Newtonsoft.Json;
    using NodaTime;

    [HideEvent]
    [EventName("Crab-MunicipalityNameWasImported")]
    [EventDescription("Legacy event om tblGemeenteNaam en tblGemeenteNaam_hist te importeren.")]
    public sealed class MunicipalityNameWasImportedFromCrab : IMessage
    {
        [EventPropertyDescription("CRAB-identificator van de gemeente.")]
        public int CrabMunicipalityId { get; }

        [EventPropertyDescription("CRAB-identificator van de gemeentenaam.")]
        public int CrabMunicipalityNameId { get; }

        [EventPropertyDescription("Officiële spelling van de gemeente.")]
        public string MunicipalityNameName { get; }

        [EventPropertyDescription("Taal waarin de officiële naam staat.")]
        public CrabLanguage? MunicipalityNameLanguage { get; }

        [EventPropertyDescription("Datum waarop het object is ontstaan in werkelijkheid.")]
        public LocalDateTime? BeginDateTime { get; }

        [EventPropertyDescription("Datum waarop het object in werkelijkheid ophoudt te bestaan.")]
        public LocalDateTime? EndDateTime { get; }

        [EventPropertyDescription("Tijdstip waarop het object werd ingevoerd in de databank.")]
        public Instant Timestamp { get; }

        [EventPropertyDescription("Operator door wie het object werd ingevoerd in de databank.")]
        public string Operator { get; }

        [EventPropertyDescription("Bewerking waarmee het object werd ingevoerd in de databank.")]
        public CrabModification? Modification { get; }

        [EventPropertyDescription("Organisatie die het object heeft ingevoerd in de databank.")]
        public CrabOrganisation? Organisation { get; }

        public MunicipalityNameWasImportedFromCrab(
            CrabMunicipalityId crabMunicipalityId,
            CrabMunicipalityNameId crabMunicipalityNameId,
            CrabMunicipalityName municipalityName,
            CrabLifetime lifetime,
            CrabTimestamp timestamp,
            CrabOperator @operator,
            CrabModification? modification,
            CrabOrganisation? organisation)
        {
            CrabMunicipalityId = crabMunicipalityId;
            CrabMunicipalityNameId = crabMunicipalityNameId;
            MunicipalityNameName = municipalityName.Name;
            MunicipalityNameLanguage = municipalityName?.Language;
            BeginDateTime = lifetime.BeginDateTime;
            EndDateTime = lifetime.EndDateTime;
            Timestamp = timestamp;
            Operator = @operator;
            Modification = modification;
            Organisation = organisation;
        }

        [JsonConstructor]
        private MunicipalityNameWasImportedFromCrab(
            int crabMunicipalityId,
            int crabMunicipalityNameId,
            string municipalityNameName,
            CrabLanguage? municipalityNameLanguage,
            LocalDateTime? beginDateTime,
            LocalDateTime? endDateTime,
            Instant timestamp,
            string @operator,
            CrabModification? modification,
            CrabOrganisation? organisation) :
            this(
                new CrabMunicipalityId(crabMunicipalityId),
                new CrabMunicipalityNameId(crabMunicipalityNameId),
                new CrabMunicipalityName(municipalityNameName, municipalityNameLanguage),
                new CrabLifetime(beginDateTime, endDateTime),
                new CrabTimestamp(timestamp),
                new CrabOperator(@operator),
                modification,
                organisation) { }
    }
}
