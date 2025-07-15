namespace MunicipalityRegistry.Producer.Extensions
{
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Contracts = Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using ContractsCommon = Be.Vlaanderen.Basisregisters.GrAr.Contracts.Common;
    using Domain = Municipality.Events;

    public static class MessageExtensions
    {
        private static ContractsCommon.Provenance ToContract(this ProvenanceData provenance) => new ContractsCommon.Provenance(
            provenance.Timestamp.ToString(),
            provenance.Application.ToString(),
            provenance.Modification.ToString(),
            provenance.Organisation.ToString(),
            provenance.Reason);

        public static Contracts.MunicipalityWasRegistered ToContract(this Domain.MunicipalityWasRegistered message) =>
            new Contracts.MunicipalityWasRegistered(message.MunicipalityId.ToString("D"), message.NisCode, message.Provenance.ToContract());

        public static Contracts.MunicipalityNisCodeWasDefined ToContract(this Domain.MunicipalityNisCodeWasDefined message) =>
            new Contracts.MunicipalityNisCodeWasDefined(message.MunicipalityId.ToString("D"), message.NisCode, message.Provenance.ToContract());

        public static Contracts.MunicipalityNisCodeWasCorrected ToContract(this Domain.MunicipalityNisCodeWasCorrected message) =>
            new Contracts.MunicipalityNisCodeWasCorrected(message.MunicipalityId.ToString("D"), message.NisCode, message.Provenance.ToContract());

        public static Contracts.MunicipalityWasNamed ToContract(this Domain.MunicipalityWasNamed message) =>
            new Contracts.MunicipalityWasNamed(message.MunicipalityId.ToString("D"), message.Name, message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityNameWasCorrected ToContract(this Domain.MunicipalityNameWasCorrected message) =>
            new Contracts.MunicipalityNameWasCorrected(message.MunicipalityId.ToString("D"), message.Name, message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityNameWasCorrectedToCleared ToContract(this Domain.MunicipalityNameWasCorrectedToCleared message) =>
            new Contracts.MunicipalityNameWasCorrectedToCleared(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityOfficialLanguageWasAdded ToContract(this Domain.MunicipalityOfficialLanguageWasAdded message) =>
            new Contracts.MunicipalityOfficialLanguageWasAdded(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityOfficialLanguageWasRemoved ToContract(this Domain.MunicipalityOfficialLanguageWasRemoved message) =>
            new Contracts.MunicipalityOfficialLanguageWasRemoved(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityFacilityLanguageWasAdded ToContract(this Domain.MunicipalityFacilityLanguageWasAdded message) =>
            new Contracts.MunicipalityFacilityLanguageWasAdded(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityFacilityLanguageWasRemoved ToContract(this Domain.MunicipalityFacilityLanguageWasRemoved message) =>
            new Contracts.MunicipalityFacilityLanguageWasRemoved(message.MunicipalityId.ToString("D"), message.Language.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityBecameCurrent ToContract(this Domain.MunicipalityBecameCurrent message) =>
            new Contracts.MunicipalityBecameCurrent(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasCorrectedToCurrent ToContract(this Domain.MunicipalityWasCorrectedToCurrent message) =>
            new Contracts.MunicipalityWasCorrectedToCurrent(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasRetired ToContract(this Domain.MunicipalityWasRetired message) =>
            new Contracts.MunicipalityWasRetired(message.MunicipalityId.ToString("D"), message.RetirementDate.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasCorrectedToRetired ToContract(this Domain.MunicipalityWasCorrectedToRetired message) =>
            new Contracts.MunicipalityWasCorrectedToRetired(message.MunicipalityId.ToString("D"), message.RetirementDate.ToString(), message.Provenance.ToContract());

        public static Contracts.MunicipalityGeometryWasCleared ToContract(this Domain.MunicipalityGeometryWasCleared message) =>
            new Contracts.MunicipalityGeometryWasCleared(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityGeometryWasCorrected ToContract(this Domain.MunicipalityGeometryWasCorrected message) =>
            new Contracts.MunicipalityGeometryWasCorrected(message.MunicipalityId.ToString("D"), message.ExtendedWkbGeometry, message.Provenance.ToContract());

        public static Contracts.MunicipalityGeometryWasCorrectedToCleared ToContract(this Domain.MunicipalityGeometryWasCorrectedToCleared message) =>
            new Contracts.MunicipalityGeometryWasCorrectedToCleared(message.MunicipalityId.ToString("D"), message.Provenance.ToContract());

        public static Contracts.MunicipalityWasDrawn ToContract(this Domain.MunicipalityWasDrawn message) =>
            new Contracts.MunicipalityWasDrawn(message.MunicipalityId.ToString("D"), message.ExtendedWkbGeometry, message.Provenance.ToContract());

        public static Contracts.MunicipalityWasMerged ToContract(this Domain.MunicipalityWasMerged message) =>
            new Contracts.MunicipalityWasMerged(
                message.MunicipalityId.ToString("D"),
                message.NisCode,
                message.MunicipalityIdsToMergeWith.Select(x => x.ToString("D")),
                message.NisCodesToMergeWith,
                message.NewMunicipalityId.ToString("D"),
                message.NewNisCode,
                message.Provenance.ToContract());

        public static Contracts.MunicipalityWasRemoved ToContract(this Domain.MunicipalityWasRemoved message) =>
            new Contracts.MunicipalityWasRemoved(message.MunicipalityId.ToString("D"), message.NisCode, message.Provenance.ToContract());
    }
}
