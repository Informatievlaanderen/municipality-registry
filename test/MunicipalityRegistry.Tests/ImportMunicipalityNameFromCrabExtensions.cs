namespace MunicipalityRegistry.Tests
{
    using Municipality.Commands.Crab;
    using Municipality.Events;

    public static class ImportMunicipalityNameFromCrabExtensions
    {
        public static MunicipalityNameWasImportedFromCrab ToLegacyEvent(
            this ImportMunicipalityNameFromCrab importMunicipalityNameFromCrab)
        {
            return new MunicipalityNameWasImportedFromCrab(
                importMunicipalityNameFromCrab.MunicipalityId,
                importMunicipalityNameFromCrab.MunicipalityNameId,
                importMunicipalityNameFromCrab.MunicipalityName,
                importMunicipalityNameFromCrab.Lifetime,
                importMunicipalityNameFromCrab.Timestamp,
                importMunicipalityNameFromCrab.Operator,
                importMunicipalityNameFromCrab.Modification,
                importMunicipalityNameFromCrab.Organisation);
        }
    }
}
