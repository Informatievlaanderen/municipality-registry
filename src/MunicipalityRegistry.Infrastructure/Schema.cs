namespace MunicipalityRegistry.Infrastructure
{
    public static class Schema
    {
        public const string Default = "MunicipalityRegistry";
        public const string Import = "MunicipalityRegistryImport";

        public const string Extract = "MunicipalityRegistryExtract";
        public const string Legacy = "MunicipalityRegistryLegacy";

        public const string Producer = "MunicipalityRegistryProducer";
    }

    public static class MigrationTables
    {
        public const string Extract = "__EFMigrationsHistoryExtract";
        public const string Legacy = "__EFMigrationsHistoryLegacy";
        public const string Producer = "__EFMigrationsHistoryProducer";
        public const string RedisDataMigration = "__EFMigrationsHistoryDataMigration";
    }
}
