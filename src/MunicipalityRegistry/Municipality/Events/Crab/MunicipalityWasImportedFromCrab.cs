namespace MunicipalityRegistry.Municipality.Events
{
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Newtonsoft.Json;
    using NodaTime;

    [EventName("Crab-MunicipalityWasImported")]
    [EventDescription("Legacy event om tblGemeente en tblGemeente_hist te importeren.")]
    public class MunicipalityWasImportedFromCrab
    {
        [EventPropertyDescription("CRAB-identificator van de gemeente.")]
        public int CrabMunicipalityId { get; }
        
        [EventPropertyDescription("NIS-code van de gemeente.")]
        public string NisCode { get; }
        
        [EventPropertyDescription("Primaire taal van de gemeente.")]
        public CrabLanguage? PrimaryLanguage { get; }
        
        [EventPropertyDescription("Secundaire taal van de gemeente.")]
        public CrabLanguage? SecondaryLanguage { get; }
        
        [EventPropertyDescription("Aantal vlagjes op dit object.")]
        public int? NumberOfFlags { get; }
        
        [EventPropertyDescription("Datum waarop het object is ontstaan in werkelijkheid.")]
        public LocalDateTime? BeginDate { get; }
        
        [EventPropertyDescription("Datum waarop het object in werkelijkheid ophoudt te bestaan.")]
        public LocalDateTime? EndDate { get; }
        
        [EventPropertyDescription("WKB-voorstelling van de gemeentegrenzen.")]
        public string WkbGeometry { get; }
        
        [EventPropertyDescription("Tijdstip waarop het object werd ingevoerd in de databank.")] 
        public Instant Timestamp { get; }
        
        [EventPropertyDescription("Operator door wie het object werd ingevoerd in de databank.")]
        public string Operator { get; }
        
        [EventPropertyDescription("Bewerking waarmee het object werd ingevoerd in de databank.")] 
        public CrabModification? Modification { get; }
        
        [EventPropertyDescription("Organisatie die het object heeft ingevoerd in de databank.")]
        public CrabOrganisation? Organisation { get; }

        public MunicipalityWasImportedFromCrab(
            CrabMunicipalityId crabMunicipalityId,
            NisCode nisCode,
            CrabLanguage? primaryLanguage,
            CrabLanguage? secondaryLanguage,
            NumberOfFlags numberOfFlags,
            CrabLifetime lifetime,
            WkbGeometry wkbGeometry,
            CrabTimestamp timestamp,
            CrabOperator @operator,
            CrabModification? modification,
            CrabOrganisation? organisation)
        {
            CrabMunicipalityId = crabMunicipalityId;
            NisCode = nisCode;
            PrimaryLanguage = primaryLanguage;
            SecondaryLanguage = secondaryLanguage;
            NumberOfFlags = numberOfFlags;
            BeginDate = lifetime.BeginDateTime;
            EndDate = lifetime.EndDateTime;
            WkbGeometry = wkbGeometry?.ToString();
            Timestamp = timestamp;
            Operator = @operator;
            Modification = modification;
            Organisation = organisation;
        }

        [JsonConstructor]
        private MunicipalityWasImportedFromCrab(
            int crabMunicipalityId,
            string nisCode,
            CrabLanguage? primaryLanguage,
            CrabLanguage? secondaryLanguage,
            int? numberOfFlags,
            LocalDateTime? beginDate,
            LocalDateTime? endDate,
            string wkbGeometry,
            Instant timestamp,
            string @operator,
            CrabModification? modification,
            CrabOrganisation? organisation) :
            this(
                new CrabMunicipalityId(crabMunicipalityId),
                new NisCode(nisCode),
                primaryLanguage,
                secondaryLanguage,
                numberOfFlags.HasValue ? new NumberOfFlags(numberOfFlags.Value) : null,
                new CrabLifetime(beginDate, endDate),
                string.IsNullOrWhiteSpace(wkbGeometry) ? null : new WkbGeometry(wkbGeometry),
                new CrabTimestamp(timestamp),
                new CrabOperator(@operator),
                modification,
                organisation) { }
    }
}
