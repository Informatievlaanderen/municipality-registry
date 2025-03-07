namespace MunicipalityRegistry.Infrastructure
{
    public static class Schema
    {
        public const string Default = "MunicipalityRegistry";
        public const string Import = "MunicipalityRegistryImport";

        public const string Extract = "MunicipalityRegistryExtract";
        public const string Legacy = "MunicipalityRegistryLegacy";

        public const string Producer = "MunicipalityRegistryProducer";
        public const string ProducerSnapshotOslo = "MunicipalityRegistryProducerSnapshotOslo";
        public const string ProducerLdes = "MunicipalityRegistryProducerLdes";
        public const string Wfs = "wfs.municipality";
        public const string Wms = "wms.municipality";
        public const string Integration = "integration_municipality";
    }

    public static class MigrationTables
    {
        public const string Extract = "__EFMigrationsHistoryExtract";
        public const string Legacy = "__EFMigrationsHistoryLegacy";
        public const string Import = "__EFMigrationsHistoryLegacy";
        public const string Producer = "__EFMigrationsHistoryProducer";
        public const string ProducerSnapshotOslo = "__EFMigrationsHistoryProducerSnapshotOslo";
        public const string ProducerLdes = "__EFMigrationsHistoryProducerLdes";
        public const string RedisDataMigration = "__EFMigrationsHistoryDataMigration";
        public const string Wfs = "__EFMigrationsHistoryWfsMunicipality";
        public const string Wms = "__EFMigrationsHistoryWmsMunicipality";
        public const string Integration = "__EFMigrationsHistory";
    }
}
