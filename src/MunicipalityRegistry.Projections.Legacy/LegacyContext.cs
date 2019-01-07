namespace MunicipalityRegistry.Projections.Legacy
{
    using System;
    using System.IO;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using MunicipalityList;

    public class LegacyContext : RunnerDbContext<LegacyContext>
    {
        public override string ProjectionStateSchema => Schema.Legacy;

        public DbSet<MunicipalityListItem> MunicipalityList { get; set; }
        public DbSet<MunicipalityDetail.MunicipalityDetail> MunicipalityDetail { get; set; }
        public DbSet<MunicipalitySyndication.MunicipalitySyndicationItem> MunicipalitySyndication { get; set; }
        public DbSet<MunicipalityName.MunicipalityName> MunicipalityName { get; set; }
        public DbSet<MunicipalityVersion.MunicipalityVersion> MunicipalityVersions { get; set; }

        // This needs to be here to please EF
        public LegacyContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public LegacyContext(DbContextOptions<LegacyContext> options)
            : base(options) { }
    }

    public class ConfigBasedContextFactory : IDesignTimeDbContextFactory<LegacyContext>
    {
        public LegacyContext CreateDbContext(string[] args)
        {
            const string migrationConnectionStringName = "LegacyProjectionsAdmin";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json")
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<LegacyContext>();

            var connectionString = configuration.GetConnectionString(migrationConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"Could not find a connection string with name '{migrationConnectionStringName}'");

            builder
                .UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();
                    sqlServerOptions.MigrationsHistoryTable(MigrationTables.Legacy, Schema.Legacy);
                });

            return new LegacyContext(builder.Options);
        }
    }
}
