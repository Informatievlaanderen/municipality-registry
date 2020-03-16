
namespace MunicipalityRegistry.Importer.Console
{
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Api;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Json;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    internal class NonBatchedHttpApiProxyFactory : IApiProxyFactory
    {
        private readonly IHttpApiProxyConfig _config;
        private readonly JsonSerializer _serializer;
        private readonly ILogger _logger;

        public NonBatchedHttpApiProxyFactory(ILogger logger, IHttpApiProxyConfig config)
        {
            _logger = logger;
            _config = config;
            _serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings().ConfigureForCrabImports());
        }

        public IApiProxy Create() => new NonBatchedHttpApiProxy(_logger, _serializer, _config);
    }
}
