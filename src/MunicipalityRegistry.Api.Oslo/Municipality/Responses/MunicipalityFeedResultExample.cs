namespace MunicipalityRegistry.Api.Oslo.Municipality.Responses
{
    using Infrastructure.Options;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using Swashbuckle.AspNetCore.Filters;

    public sealed class MunicipalityFeedResultExample : IExamplesProvider<object>
    {
        private readonly ResponseOptions _feedConfig;

        public MunicipalityFeedResultExample(IOptions<ResponseOptions> feedConfig)
        {
            _feedConfig = feedConfig.Value;
        }

        public object GetExamples()
        {
            var json = $$"""
                         [
                             {
                                 "specversion": "1.0",
                                 "id": "5",
                                 "time": "2002-08-13T17:32:32+02:00",
                                 "type": "basisregisters.municipality.create.v1",
                                 "source": "{{_feedConfig.MunicipalityFeed.FeedUrl}}",
                                 "datacontenttype": "application/json",
                                 "dataschema": "{{_feedConfig.MunicipalityFeed.DataSchemaUrl}}",
                                 "basisregisterseventtype": "MunicipalityWasRegistered",
                                 "basisregisterscausationid": "d1c6eec2-a1ae-5c24-95f3-5f9c484305db",
                                 "data": {
                                     "@id": "https://data.vlaanderen.be/id/gemeente/11001",
                                     "objectId": "11001",
                                     "naamruimte": "https://data.vlaanderen.be/id/gemeente",
                                     "versieId": "2002-08-13T15:32:32Z",
                                     "nisCodes": [
                                         "11001"
                                     ],
                                     "attributen": [
                                         {
                                             "naam": "nisCode",
                                             "oudeWaarde": null,
                                             "nieuweWaarde": "11001"
                                         },
                                         {
                                             "naam": "gemeenteStatus",
                                             "oudeWaarde": null,
                                             "nieuweWaarde": "voorgesteld"
                                         }
                                     ]
                                 }
                             },
                             {
                                 "specversion": "1.0",
                                 "id": "6",
                                 "time": "2002-08-13T17:32:32+02:00",
                                 "type": "basisregisters.municipality.update.v1",
                                 "source": "{{_feedConfig.MunicipalityFeed.FeedUrl}}",
                                 "datacontenttype": "application/json",
                                 "dataschema": "{{_feedConfig.MunicipalityFeed.DataSchemaUrl}}",
                                 "basisregisterseventtype": "MunicipalityOfficialLanguageWasAdded",
                                 "basisregisterscausationid": "d1c6eec2-a1ae-5c24-95f3-5f9c484305db",
                                 "data": {
                                     "@id": "https://data.vlaanderen.be/id/gemeente/11001",
                                     "objectId": "11001",
                                     "naamruimte": "https://data.vlaanderen.be/id/gemeente",
                                     "versieId": "2002-08-13T15:32:32Z",
                                     "nisCodes": [
                                         "11001"
                                     ],
                                     "attributen": [
                                         {
                                             "naam": "officieleTalen",
                                             "oudeWaarde": [],
                                             "nieuweWaarde": [
                                                 "nl"
                                             ]
                                         }
                                     ]
                                 }
                             }
                         ]
                         """;
            return JArray.Parse(json);
        }
    }
}
