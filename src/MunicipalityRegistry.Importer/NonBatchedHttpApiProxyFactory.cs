namespace MunicipalityRegistry.Importer
{
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Api;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Json;
    using Newtonsoft.Json;

    internal class NonBatchedHttpApiProxyFactory : IApiProxyFactory
    {
        private readonly IHttpApiProxyConfig _config;
        private readonly JsonSerializer _serializer;

        public NonBatchedHttpApiProxyFactory(IHttpApiProxyConfig config)
        {
            _config = config;
            _serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings().ConfigureForCrabImports());
        }

        public IApiProxy Create() => new NonBatchedHttpApiProxy(_serializer, _config);
    }
}
