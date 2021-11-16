namespace MunicipalityRegistry.Projector.Infrastructure.Modules
{
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac;
    using Be.Vlaanderen.Basisregisters.Projector;
    using Be.Vlaanderen.Basisregisters.Projector.ConnectedProjections;
    using Be.Vlaanderen.Basisregisters.Projector.Modules;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MunicipalityRegistry.Infrastructure;
    using MunicipalityRegistry.Projections.Extract;
    using MunicipalityRegistry.Projections.Extract.MunicipalityExtract;
    using MunicipalityRegistry.Projections.LastChangedList;
    using MunicipalityRegistry.Projections.Legacy;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityList;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityName;
    using MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication;
    using MunicipalityRegistry.Projections.QueuePublisher;
    using MunicipalityRegistry.Projections.QueuePublisher.MessageDetail;
    using LastChangedListContextMigrationFactory = MunicipalityRegistry.Projections.LastChangedList.LastChangedListContextMigrationFactory;

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
            builder.RegisterModule(new DataDogModule(_configuration));

            RegisterProjectionSetup(builder);

            builder
                .RegisterType<ProblemDetailsHelper>()
                .AsSelf();

            builder.Populate(_services);
        }

        private void RegisterProjectionSetup(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new EventHandlingModule(
                        typeof(DomainAssemblyMarker).Assembly,
                        EventsJsonSerializerSettingsProvider.CreateSerializerSettings()))

                .RegisterModule<EnvelopeModule>()

                .RegisterEventstreamModule(_configuration)

                .RegisterModule(new ProjectorModule(_configuration));

            RegisterExtractProjections(builder);
            RegisterLastChangedProjections(builder);
            RegisterLegacyProjections(builder);
            RegisterQueuePublisherProjections(builder);
        }

        private void RegisterExtractProjections(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new ExtractModule(
                    _configuration,
                    _services,
                    _loggerFactory));

            builder
                .RegisterProjectionMigrator<ExtractContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<MunicipalityExtractProjections, ExtractContext>(
                    context => new MunicipalityExtractProjections(context.Resolve<IOptions<ExtractConfig>>(), DbaseCodePage.Western_European_ANSI.ToEncoding()), ConnectedProjectionSettings.Default);
        }

        private void RegisterLastChangedProjections(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new LastChangedListModule(
                    _configuration.GetConnectionString("LastChangedList"),
                    _configuration["DataDog:ServiceName"],
                    _services,
                    _loggerFactory));

            builder
                .RegisterProjectionMigrator<LastChangedListContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<LastChangedListProjections, LastChangedListContext>(ConnectedProjectionSettings.Default);
        }

        private void RegisterLegacyProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new LegacyModule(
                        _configuration,
                        _services,
                        _loggerFactory));
            builder
                .RegisterProjectionMigrator<LegacyContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<MunicipalityDetailProjections, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<MunicipalityListProjections, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<MunicipalityNameProjections, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<MunicipalitySyndicationProjections, LegacyContext>(ConnectedProjectionSettings.Default);
        }

        private void RegisterQueuePublisherProjections(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new QueuePublisherModule(
                    _configuration,
                    _services,
                    _loggerFactory));

            builder
                .RegisterProjectionMigrator<QueuePublisherContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<MessageDetailProjections, QueuePublisherContext>(ConnectedProjectionSettings.Default);
        }
    }
}
