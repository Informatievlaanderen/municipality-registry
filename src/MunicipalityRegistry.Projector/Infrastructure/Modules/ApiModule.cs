namespace MunicipalityRegistry.Projector.Infrastructure.Modules
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac;
    using Be.Vlaanderen.Basisregisters.Projector;
    using Be.Vlaanderen.Basisregisters.Projector.ConnectedProjections;
    using Be.Vlaanderen.Basisregisters.Projector.Modules;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MunicipalityRegistry.Infrastructure;
    using MunicipalityRegistry.Projections.Extract;
    using MunicipalityRegistry.Projections.Extract.MunicipalityExtract;
    using MunicipalityRegistry.Projections.Feed;
    using MunicipalityRegistry.Projections.Feed.MunicipalityFeed;
    using MunicipalityRegistry.Projections.Integration;
    using MunicipalityRegistry.Projections.Integration.Infrastructure;
    using MunicipalityRegistry.Projections.LastChangedList;
    using MunicipalityRegistry.Projections.Legacy;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityList;
    using MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication;
    using MunicipalityRegistry.Projections.Wfs;
    using MunicipalityRegistry.Projections.Wfs.Municipality;
    using MunicipalityRegistry.Projections.Wms;
    using Newtonsoft.Json;
    using LastChangedListContextMigrationFactory =
        MunicipalityRegistry.Projections.LastChangedList.LastChangedListContextMigrationFactory;

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
            RegisterWfsProjections(builder);
            RegisterWmsProjections(builder);
            RegisterFeedProjections(builder);

            if(_configuration.GetSection("Integration").GetValue("Enabled", false))
                RegisterIntegrationProjections(builder);
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
                    context => new MunicipalityExtractProjections(context.Resolve<IOptions<ExtractConfig>>(),
                        DbaseCodePage.Western_European_ANSI.ToEncoding()), ConnectedProjectionSettings.Default);
        }

        private void RegisterLastChangedProjections(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new MunicipalityLastChangedListModule(
                    _configuration.GetConnectionString("LastChangedList"),
                    _services,
                    _loggerFactory));

            builder
                .RegisterProjectionMigrator<LastChangedListContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjectionMigrator<DataMigrationContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<LastChangedListProjections, LastChangedListContext>(ConnectedProjectionSettings
                    .Default);
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
                .RegisterProjections<MunicipalitySyndicationProjections, LegacyContext>(ConnectedProjectionSettings
                    .Default);
        }

        private void RegisterFeedProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new FeedModule(
                        _configuration,
                        _services,
                        _loggerFactory,
                        new JsonSerializerSettings().ConfigureDefaultForApi()));
            builder
                .RegisterProjectionMigrator<FeedContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<MunicipalityFeedProjections, FeedContext>(context =>
                    new MunicipalityFeedProjections(
                        context.Resolve<IOptions<MunicipalityFeedConfig>>().Value,
                        context.Resolve<LastChangedListContext>(),
                        new JsonSerializerSettings().ConfigureDefaultForApi()),
                    ConnectedProjectionSettings.Configure(c =>
                    {
                        c.ConfigureCatchUpPageSize(1);
                        c.ConfigureCatchUpUpdatePositionMessageInterval(1);
                    }));
        }

        private void RegisterIntegrationProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new IntegrationModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            builder
                .RegisterProjectionMigrator<IntegrationContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<MunicipalityLatestItemProjections, IntegrationContext>(
                    context => new MunicipalityLatestItemProjections(context.Resolve<IOptions<IntegrationOptions>>()),
                    ConnectedProjectionSettings.Default)
                .RegisterProjections<MunicipalityVersionProjections, IntegrationContext>(
                    context => new MunicipalityVersionProjections(context.Resolve<IOptions<IntegrationOptions>>()),
                    ConnectedProjectionSettings.Default);
        }

        private void RegisterWfsProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new WfsModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            var wfsProjectionSettings = ConnectedProjectionSettings
                .Configure(settings =>
                    settings.ConfigureLinearBackoff<SqlException>(_configuration, "Wfs"));

            builder
                .RegisterProjectionMigrator<WfsContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<MunicipalityHelperProjections, WfsContext>(() =>
                        new MunicipalityHelperProjections(),
                    wfsProjectionSettings);
        }

        private void RegisterWmsProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new WmsModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            var wmsProjectionSettings = ConnectedProjectionSettings
                .Configure(settings =>
                    settings.ConfigureLinearBackoff<SqlException>(_configuration, "Wms"));

            builder
                .RegisterProjectionMigrator<WmsContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<MunicipalityRegistry.Projections.Wms.Municipality.MunicipalityHelperProjections,
                    WmsContext>(() =>
                        new MunicipalityRegistry.Projections.Wms.Municipality.MunicipalityHelperProjections(),
                    wmsProjectionSettings);
        }
    }
}
