namespace MunicipalityRegistry.Producer.Extensions
{
    using System.Globalization;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Contracts = Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using ContractsCommon = Be.Vlaanderen.Basisregisters.GrAr.Contracts.Common;
    using Muni = Municipality.Events;

    public static class MessageExtensions
    {
        private static string ToIsoString(this NodaTime.Instant instant) => instant.ToString("uuuu-MM-ddTHH:mm:ss'Z", CultureInfo.InvariantCulture);

        private static ContractsCommon.Provenance ToContract(this ProvenanceData provenance) => new ContractsCommon.Provenance(
            provenance.Timestamp.ToIsoString(),
            provenance.Application.ToString(),
            provenance.Modification.ToString(),
            provenance.Operator,
            provenance.Organisation.ToString(),
            provenance.Reason);

        public static Contracts.MunicipalityWasRegistered ToContract(this Muni.MunicipalityWasRegistered message) =>
            new Contracts.MunicipalityWasRegistered(message.MunicipalityId.ToString("D"), message.NisCode, message.Provenance.ToContract());

        public static Contracts.MunicipalityNisCodeWasDefined ToContract(this Muni.MunicipalityNisCodeWasDefined message) =>
            new Contracts.MunicipalityNisCodeWasDefined(message.MunicipalityId.ToString("D"), message.NisCode, message.Provenance.ToContract());

        public static Contracts.MunicipalityNisCodeWasCorrected ToContract(this Muni.MunicipalityNisCodeWasCorrected message) =>
            new Contracts.MunicipalityNisCodeWasCorrected(message.MunicipalityId.ToString("D"), message.NisCode, message.Provenance.ToContract());

        public static Contracts.MunicipalityWasNamed ToContract(this Muni.MunicipalityWasNamed message) =>
            new Contracts.MunicipalityWasNamed(message.MunicipalityId.ToString("D"), message.Name, message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityNameWasCorrected ToContract(this Muni.MunicipalityNameWasCorrected message) =>
            new Contracts.MunicipalityNameWasCorrected(message.MunicipalityId.ToString("D"), message.Name, message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityNameWasCorrectedToCleared ToContract(this Muni.MunicipalityNameWasCorrectedToCleared message) =>
            new Contracts.MunicipalityNameWasCorrectedToCleared(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityOfficialLanguageWasAdded ToContract(this Muni.MunicipalityOfficialLanguageWasAdded message) =>
            new Contracts.MunicipalityOfficialLanguageWasAdded(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityOfficialLanguageWasRemoved ToContract(this Muni.MunicipalityOfficialLanguageWasRemoved message) =>
            new Contracts.MunicipalityOfficialLanguageWasRemoved(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityFacilityLanguageWasAdded ToContract(this Muni.MunicipalityFacilityLanguageWasAdded message) =>
            new Contracts.MunicipalityFacilityLanguageWasAdded(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityFacilitiesLanguageWasRemoved ToContract(this Muni.MunicipalityFacilitiesLanguageWasRemoved message) =>
            new Contracts.MunicipalityFacilitiesLanguageWasRemoved(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityBecameCurrent ToContract(this Muni.MunicipalityBecameCurrent message) =>
            new Contracts.MunicipalityBecameCurrent(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasCorrectedToCurrent ToContract(this Muni.MunicipalityWasCorrectedToCurrent message) =>
            new Contracts.MunicipalityWasCorrectedToCurrent(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasRetired ToContract(this Muni.MunicipalityWasRetired message) =>
            new Contracts.MunicipalityWasRetired(message.MunicipalityId.ToString("D"), message.RetirementDate.ToIsoString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasCorrectedToRetired ToContract(this Muni.MunicipalityWasCorrectedToRetired message) =>
            new Contracts.MunicipalityWasCorrectedToRetired(message.MunicipalityId.ToString("D"), message.RetirementDate.ToIsoString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityGeometryWasCleared ToContract(this Muni.MunicipalityGeometryWasCleared message) =>
            new Contracts.MunicipalityGeometryWasCleared(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityGeometryWasCorrected ToContract(this Muni.MunicipalityGeometryWasCorrected message) =>
            new Contracts.MunicipalityGeometryWasCorrected(message.MunicipalityId.ToString("D"), message.ExtendedWkbGeometry, message.Provenance.ToContract());

        public static Contracts.MunicipalityGeometryWasCorrectedToCleared ToContract(this Muni.MunicipalityGeometryWasCorrectedToCleared message) =>
            new Contracts.MunicipalityGeometryWasCorrectedToCleared(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasDrawn ToContract(this Muni.MunicipalityWasDrawn message) =>
            new Contracts.MunicipalityWasDrawn(message.MunicipalityId.ToString("D"), message.ExtendedWkbGeometry, message.Provenance.ToContract());
    }
}
