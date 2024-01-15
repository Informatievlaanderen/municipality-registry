namespace MunicipalityRegistry.GeometryImporter
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Destructurama;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Projections.Integration;
    using Serilog;
    using Serilog.Debugging;

    public sealed class Program
    {
        private Program()
        { }

        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
                Log.Debug(
                    eventArgs.Exception,
                    "FirstChanceException event raised in {AppDomain}.",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
                Log.Fatal((Exception) eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

            Log.Information("Initializing Municipality Geometry Importer");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var services = new ServiceCollection();

            var connectionString = configuration.GetConnectionString("IntegrationDb")
                                   ?? throw new InvalidOperationException(
                                       $"Could not find a connection string with name 'IntegrationDb'");

            services
                .AddDbContext<IntegrationContext>((_, options) =>
                {
                    options.UseLoggerFactory(new NullLoggerFactory());//provider.GetRequiredService<ILoggerFactory>());
                    options.UseNpgsql(connectionString, sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                        sqlServerOptions.UseNetTopologySuite();
                    });
                });

            SelfLog.Enable(Console.WriteLine);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentUserName()
                .Destructure.JsonNetTypes()
                .CreateLogger();

            services.AddLogging(l =>
            {
                l.ClearProviders();
                l.AddSerilog(Log.Logger);
                l.SetMinimumLevel(LogLevel.Warning);
            });

            var container = services.BuildServiceProvider();
            Log.Information("Starting  Municipality Geometry Importer");

            var importer = new Importer(container.GetRequiredService<IntegrationContext>());
            await importer.ExecuteAsync();

            Log.Information("Stopping  Municipality Geometry Importer");
        }
    }
}
