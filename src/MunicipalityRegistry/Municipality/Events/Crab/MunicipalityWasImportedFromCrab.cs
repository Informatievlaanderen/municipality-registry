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
        public int CrabMunicipalityId { get; }
        public string NisCode { get; }
        public CrabLanguage? PrimaryLanguage { get; }
        public CrabLanguage? SecondaryLanguage { get; }
        public int? NumberOfFlags { get; }
        public LocalDateTime? BeginDate { get; }
        public LocalDateTime? EndDate { get; }
        public string WkbGeometry { get; }
        public Instant Timestamp { get; }
        public string Operator { get; }
        public CrabModification? Modification { get; }
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
