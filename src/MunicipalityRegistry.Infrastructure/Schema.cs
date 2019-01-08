namespace MunicipalityRegistry.Infrastructure
{
    public class Schema
    {
        public const string Default = "MunicipalityRegistry";

        public const string Extract = "MunicipalityRegistryExtract";
        public const string Legacy = "MunicipalityRegistryLegacy";
    }

    public class MigrationTables
    {
        public const string Extract = "__EFMigrationsHistoryExtract";
        public const string Legacy = "__EFMigrationsHistoryLegacy";
    }
}
