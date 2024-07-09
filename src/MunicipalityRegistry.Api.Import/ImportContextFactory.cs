namespace MunicipalityRegistry.Api.Import
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using MunicipalityRegistry.Infrastructure;

    public sealed class ImportContextFactory : IDesignTimeDbContextFactory<ImportContext>
    {
        public ImportContext CreateDbContext(string[] args)
        {
            const string migrationConnectionStringName = "Import";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<ImportContext>();

            var connectionString = configuration.GetConnectionString(migrationConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"Could not find a connection string with name '{migrationConnectionStringName}'");

            builder
                .UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();
                    sqlServerOptions.MigrationsHistoryTable(MigrationTables.Import, Schema.Import);
                });

            return new ImportContext(builder.Options);
        }
    }
}
