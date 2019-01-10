namespace MunicipalityRegistry.Importer
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Api;
    using Newtonsoft.Json;

    public class NonBatchedHttpApiProxy : IApiProxy
    {
        private readonly IHttpApiProxyConfig _config;
        private readonly JsonSerializer _serializer;

        public NonBatchedHttpApiProxy(
            JsonSerializer serializer,
            IHttpApiProxyConfig config)
        {
            _serializer = serializer;
            _config = config;
        }

        public void ImportBatch<TKey>(IEnumerable<KeyImport<TKey>> imports)
        {
            using (var client = new HttpClient {BaseAddress = _config.BaseUrl})
            {
                client.Timeout = TimeSpan.FromMinutes(_config.HttpTimeoutMinutes);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(_config.AuthUserName) &&
                    !string.IsNullOrEmpty(_config.AuthPassword))
                {
                    var encodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.AuthUserName}:{_config.AuthPassword}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedString);
                }

                foreach (var import in imports)
                foreach (var command in import.Commands)
                {
                    var json = _serializer.Serialize(command);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(_config.ImportEndpoint, content).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}
