namespace MunicipalityRegistry.Tests
{
    using Municipality.Commands.Crab;
    using Municipality.Events;

    public static class ImportMunicipalityFromCrabExtensions
    {
        public static MunicipalityWasImportedFromCrab ToLegacyEvent(
            this ImportMunicipalityFromCrab importMunicipalityFromCrab)
        {
            return new MunicipalityWasImportedFromCrab(
                importMunicipalityFromCrab.MunicipalityId,
                importMunicipalityFromCrab.NisCode,
                importMunicipalityFromCrab.PrimaryLanguage,
                importMunicipalityFromCrab.SecondaryLanguage,
                importMunicipalityFromCrab.NumberOfFlags,
                importMunicipalityFromCrab.Lifetime,
                importMunicipalityFromCrab.Geometry,
                importMunicipalityFromCrab.Timestamp,
                importMunicipalityFromCrab.Operator,
                importMunicipalityFromCrab.Modification,
                importMunicipalityFromCrab.Organisation);
        }
    }
}
