namespace MunicipalityRegistry.Projections.Legacy
{
    using System;
    using System.IO;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.MigrationExtensions;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using MunicipalityList;
    using MunicipalitySyndication;

    public class LegacyContext : RunnerDbContext<LegacyContext>
    {
        public override string ProjectionStateSchema => Schema.Legacy;

        public DbSet<MunicipalityListItem> MunicipalityList { get; set; }
        public DbSet<MunicipalityDetail.MunicipalityDetail> MunicipalityDetail { get; set; }
        public DbSet<MunicipalityVersion.MunicipalityVersion> MunicipalityVersions { get; set; }
        public DbSet<MunicipalityName.MunicipalityName> MunicipalityName { get; set; }
        public DbSet<MunicipalitySyndicationItem> MunicipalitySyndication { get; set; }

        // This needs to be here to please EF
        public LegacyContext()
        {
        }

        // This needs to be DbContextOptions<T> for Autofac!
        public LegacyContext(DbContextOptions<LegacyContext> options)
            : base(options)
        {
        }
    }

    public class ConfigBasedContextFactory : IDesignTimeDbContextFactory<LegacyContext>
    {
        public LegacyContext CreateDbContext(string[] args)
        {
            const string migrationConnectionStringName = "LegacyProjectionsAdmin";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true,
                    reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(migrationConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(
                    $"Could not find a connection string with name '{migrationConnectionStringName}'");

            var builder = new DbContextOptionsBuilder<LegacyContext>()
                .UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();
                    sqlServerOptions.MigrationsHistoryTable(MigrationTables.Legacy, Schema.Legacy);
                })
                .UseExtendedSqlServerMigrations();

            return new LegacyContext(builder.Options);
        }
    }
}
