using Microsoft.Extensions.Logging;

namespace MunicipalityRegistry.Importer
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Api;
    using Newtonsoft.Json;

    public class NonBatchedHttpApiProxy : HttpApiProxyBase
    {
        public NonBatchedHttpApiProxy(
            ILogger logger,
            JsonSerializer serializer,
            IHttpApiProxyConfig config)
            : base(logger, serializer, config)
        { }

        public override void ImportBatch<TKey>(IEnumerable<KeyImport<TKey>> imports)
        {
            using (var client = CreateImportClient())
            {
                foreach (var import in imports)
                foreach (var command in import.Commands)
                {
                    client
                        .PostAsync(
                            Config.ImportEndpoint,
                            CreateJsonContent(Serializer.Serialize(command)))
                        .GetAwaiter()
                        .GetResult()
                        .EnsureSuccessStatusCode();
                }
            }
        }
    }
}
