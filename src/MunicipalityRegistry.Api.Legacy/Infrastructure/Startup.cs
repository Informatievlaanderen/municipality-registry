namespace MunicipalityRegistry.Api.Legacy.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Modules;
    using Options;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private IContainer _applicationContainer;

        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        /// <summary>Configures services for the application.</summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureDefaultForApi<Startup>(
                    new StartupConfigureOptions
                    {
                        Cors =
                        {
                            Origins = _configuration
                                .GetSection("Cors")
                                .GetChildren()
                                .Select(c => c.Value)
                                .ToArray()
                        },
                        Swagger =
                        {
                            ApiInfo = (provider, description) => new Info
                            {
                                Version = description.ApiVersion.ToString(),
                                Title = "Basisregisters Vlaanderen Municipality Registry API",
                                Description = GetApiLeadingText(description),
                                Contact = new Contact
                                {
                                    Name = "Informatie Vlaanderen",
                                    Email = "informatie.vlaanderen@vlaanderen.be",
                                    Url = "https://legacy.basisregisters.vlaanderen"
                                }
                            },
                            XmlCommentPaths = new[] { typeof(Startup).GetTypeInfo().Assembly.GetName().Name }
                        }
                    })
                .Configure<ResponseOptions>(_configuration);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new ApiModule(_configuration, services, _loggerFactory));
            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider,
            ApiDataDogToggle datadogToggle,
            ApiDebugDataDogToggle debugDataDogToggle)
        {
            app
                .UseDatadog<Startup>(
                    serviceProvider,
                    loggerFactory,
                    datadogToggle,
                    debugDataDogToggle,
                    _configuration["DataDog:ServiceName"])
                .UseDefaultForApi(new StartupUseOptions
                {
                    Common =
                    {
                        ApplicationContainer = _applicationContainer,
                        ServiceProvider = serviceProvider,
                        HostingEnvironment = env,
                        ApplicationLifetime = appLifetime,
                        LoggerFactory = loggerFactory
                    },
                    Api =
                    {
                        VersionProvider = apiVersionProvider,
                        Info = groupName => $"Basisregisters Vlaanderen - Municipality Registry API {groupName}"
                    },
                    MiddlewareHooks =
                    {
                        AfterMiddleware = x => x.UseMiddleware<AddNoCacheHeadersMiddleware>(),
                    }
                });
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
            => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Municipality Registry API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
    }
}
