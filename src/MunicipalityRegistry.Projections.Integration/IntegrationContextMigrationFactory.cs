namespace MunicipalityRegistry.Projections.Integration
{
    using System;
    using System.IO;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Npgsql;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

    public class IntegrationContextMigrationFactory : NpgsqlRunnerDbContextMigrationFactory<IntegrationContext>
    {
        public IntegrationContextMigrationFactory()
            : base("IntegrationProjectionsAdmin", HistoryConfiguration) { }

        private static MigrationHistoryConfiguration HistoryConfiguration =>
            new MigrationHistoryConfiguration
            {
                Schema = Schema.Integration,
                Table = MigrationTables.Integration
            };

        protected override IntegrationContext CreateContext(DbContextOptions<IntegrationContext> migrationContextOptions)
            => new IntegrationContext(migrationContextOptions);
    }
}
