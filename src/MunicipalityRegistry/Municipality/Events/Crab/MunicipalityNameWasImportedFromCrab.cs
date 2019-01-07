namespace MunicipalityRegistry.Municipality.Events
{
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Newtonsoft.Json;
    using NodaTime;

    [EventName("Crab-MunicipalityNameWasImported")]
    [EventDescription("Legacy event om tblGemeenteNaam en tblGemeenteNaam_hist te importeren.")]
    public class MunicipalityNameWasImportedFromCrab
    {
        public int CrabMunicipalityId { get; }
        public int CrabMunicipalityNameId { get; }
        public string MunicipalityNameName { get; }
        public CrabLanguage? MunicipalityNameLanguage { get; }
        public LocalDateTime? BeginDateTime { get; }
        public LocalDateTime? EndDateTime { get; }
        public Instant Timestamp { get; }
        public string Operator { get; }
        public CrabModification? Modification { get; }
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
            MunicipalityNameName = municipalityName?.Name;
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
                organisation
            ) { }
    }
}
