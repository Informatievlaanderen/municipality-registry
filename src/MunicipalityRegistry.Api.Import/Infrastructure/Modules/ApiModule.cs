namespace MunicipalityRegistry.Api.Import.Infrastructure.Modules
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MunicipalityRegistry.Infrastructure;
    using MunicipalityRegistry.Infrastructure.Modules;
    using Projections.Legacy;
    using Vrbg;

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

        protected override void Load(ContainerBuilder builder)
        {
            var eventSerializerSettings = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();

            builder
                .RegisterModule(new EventHandlingModule(typeof(DomainAssemblyMarker).Assembly, eventSerializerSettings))
                .RegisterModule(new EnvelopeModule())
                .RegisterModule(new CommandHandlingModule(_configuration))
                .RegisterModule(new ImportModule(_configuration, _services, _loggerFactory))
                .RegisterModule(new LegacyModule(_configuration, _services, _loggerFactory));

            _services.ConfigureIdempotency(
                _configuration.GetSection(IdempotencyConfiguration.Section).Get<IdempotencyConfiguration>().ConnectionString,
                new IdempotencyMigrationsTableInfo(Schema.Import),
                new IdempotencyTableInfo(Schema.Import),
                _loggerFactory);

            builder
                .RegisterType<ProblemDetailsHelper>()
                .AsSelf();

            builder
                .RegisterType<VrbgGeometryService>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Populate(_services);
        }
    }
}
