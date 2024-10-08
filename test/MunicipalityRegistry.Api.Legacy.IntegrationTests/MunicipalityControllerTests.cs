namespace MunicipalityRegistry.Api.Legacy.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using MunicipalityRegistry.Api.IntegrationTests;
    using Newtonsoft.Json;
    using Oslo.Infrastructure;
    using Oslo.Municipality.Responses;
    using Xunit;

    public class MunicipalityControllerTests
    {
        [Fact(Skip = "Deserialization error on GemeenteIdenificator")]
        public async Task List()
        {
            var application = new WebApplicationFactory<Program>();

            int? offset = 0;
            int? limit = 100;

            var client = application.CreateClient();

            // filtering
            client.DefaultRequestHeaders.Add("X-Filtering", JsonConvert.SerializeObject(new
            {
                Status = "InGebruik",
                IsFlemishRegion = true
            }));

            var result = new List<MunicipalityListOsloItemResponse>();

            const string url = "/v2/gemeenten";
            while (true)
            {
                // pagination
                client.SetPaginationHeader("X-Pagination", offset, limit);

                var response = await client.GetJsonAsync<MunicipalityListOsloResponse>(url);
                if (response != null)
                {
                    result.AddRange(response.Gemeenten);

                    if (!response.Volgende.IsValid())
                    {
                        break;
                    }

                    if (response.Volgende != null)
                    {
                        (offset, limit) = response.Volgende.PathAndQuery.ParsePaginationFromUrl();
                    }
                }
            }

            Assert.NotEmpty(result);
            Assert.DoesNotContain(result, x => x.Gemeentenaam.GeografischeNaam.Spelling.Equals("Ath", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
