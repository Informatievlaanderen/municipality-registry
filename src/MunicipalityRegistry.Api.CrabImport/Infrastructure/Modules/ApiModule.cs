namespace MunicipalityRegistry.Api.CrabImport.Infrastructure.Modules
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.CrabImport;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MunicipalityRegistry.Infrastructure;
    using MunicipalityRegistry.Infrastructure.Modules;

    public class ApiModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public ApiModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            var eventSerializerSettings = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();

            containerBuilder
                .RegisterModule(new DataDogModule(_configuration))

                .RegisterModule(new IdempotencyModule(
                    _services,
                    _configuration.GetSection(IdempotencyConfiguration.Section).Get<IdempotencyConfiguration>().ConnectionString,
                    new IdempotencyMigrationsTableInfo(Schema.Default),
                    new IdempotencyTableInfo(Schema.Default),
                    _loggerFactory))

                .RegisterModule(new EventHandlingModule(typeof(DomainAssemblyMarker).Assembly, eventSerializerSettings))

                .RegisterModule(new EnvelopeModule())

                .RegisterModule(new CommandHandlingModule(_configuration))

                .RegisterModule(new CrabImportModule(
                    _services,
                    _configuration.GetConnectionString("CrabImport"),
                    Schema.Default,
                    "__EFMigrationsHistoryCrabImport", // remove on upgrade to GrAr.Common 7.2, default HistoryTableName is set in package
                    _loggerFactory));

            containerBuilder.Populate(_services);
        }
    }
}
