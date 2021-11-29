namespace MunicipalityRegistry.Api.Oslo.Municipality.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.JsonConverters;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [DataContract(Name = "GemeenteDetail", Namespace = "")]
    public class MunicipalityOsloResponse
    {
        /// <summary>
        /// De linked-data context van gemeente.
        /// </summary>
        [DataMember(Name = "@context", Order = 0)]
        [JsonProperty(Required = Required.DisallowNull)]
        [JsonConverter(typeof(PlainStringJsonConverter))]
        public object Context => @"{ 
       ""@base"":""https://data.vlaanderen.be/id/concept/"",
       ""identificator"": ""@nest"",
        ""id"": ""@id"",
        ""versieId"": {
          ""@id"": ""https://data.vlaanderen.be/ns/generiek#versieIdentificator"",
          ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
         },   
        ""gemeentenamen"" : {
            ""@id"": ""http://www.w3.org/2000/01/rdf-schema#label"",
            ""@context"": {
              ""spelling"": ""@value"",
              ""taal"": ""@language""
             }
        },
        ""gemeenteStatus"": {
          ""@id"": ""https://data.vlaanderen.be/ns/adres#Gemeente.status"",
          ""@type"": ""@id"",
          ""@context"": {
            ""@base"": ""https://data.vlaanderen.be/id/concept/gemeentestatus/""
          }
        },
        ""officieleTalen"": {
          ""@id"": ""http://www.w3.org/2000/01/rdf-schema#label""
        },
        ""faciliteitenTalen"": {
          ""@id"": ""http://www.w3.org/2000/01/rdf-schema#label""
        }
    }";

        /// <summary>
        /// Het linked-data type van de gemeente.
        /// </summary>
        [DataMember(Name = "@type", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Type => "Gemeente";

        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [DataMember(Name = "Identificator", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public GemeenteIdentificator Identificator { get; private set; }

        /// <summary>
        /// De officiële talen van de gemeente.
        /// </summary>
        [DataMember(Name = "OfficieleTalen", Order = 3)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<Taal> OfficialLanguages { get; private set; }

        /// <summary>
        /// De faciliteiten talen van de gemeente.
        /// </summary>
        [DataMember(Name = "FaciliteitenTalen", Order = 4)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<Taal> FacilitiesLanguages { get; private set; }

        /// <summary>
        /// De officiële namen van de gemeente.
        /// </summary>
        [DataMember(Name = "Gemeentenamen", Order = 5)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<GeografischeNaam> Gemeentenamen { get; private set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [DataMember(Name = "GemeenteStatus", Order = 6)]
        [JsonProperty(Required = Required.DisallowNull)]
        public GemeenteStatus GemeenteStatus { get; private set; }

        public MunicipalityOsloResponse(
            string naamruimte,
            GemeenteStatus status,
            string nisCode,
            IEnumerable<Language> officialLanguages,
            IEnumerable<Language> facilitiesLanguages,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish,
            DateTimeOffset version)
        {
            Identificator = new GemeenteIdentificator(naamruimte, nisCode, version);
            GemeenteStatus = status;
            OfficialLanguages = officialLanguages.Select(LanguageExtensions.ConvertFromLanguage).ToList();
            FacilitiesLanguages = facilitiesLanguages.Select(LanguageExtensions.ConvertFromLanguage).ToList();

            var gemeenteNamen = new List<GeografischeNaam>
            {
                new GeografischeNaam(nameDutch, Taal.NL),
                new GeografischeNaam(nameFrench, Taal.FR),
                new GeografischeNaam(nameGerman, Taal.DE),
                new GeografischeNaam(nameEnglish, Taal.EN)
            };

            Gemeentenamen = gemeenteNamen.Where(x => !string.IsNullOrEmpty(x.Spelling)).ToList();
        }
    }

    public class MunicipalityOsloResponseExamples : IExamplesProvider<MunicipalityOsloResponse>
    {
        private readonly ResponseOptions _responseOptions;

        public MunicipalityOsloResponseExamples(IOptions<ResponseOptions> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public MunicipalityOsloResponse GetExamples()
            => new MunicipalityOsloResponse(
                _responseOptions.Naamruimte,
                GemeenteStatus.InGebruik,
                "31005",
                new List<Language> { Language.Dutch },
                new List<Language> { Language.French },
                "Brugge",
                "Bruges",
                "Brügge",
                "Bruges",
                DateTimeOffset.Now.ToExampleOffset());
    }

    public class MunicipalityNotFoundResponseExamples : IExamplesProvider<ProblemDetails>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProblemDetailsHelper _problemDetailsHelper;

        public MunicipalityNotFoundResponseExamples(
            IHttpContextAccessor httpContextAccessor,
            ProblemDetailsHelper problemDetailsHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _problemDetailsHelper = problemDetailsHelper;
        }

        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                ProblemTypeUri = "urn:be.vlaanderen.basisregisters.api:municipality:not-found",
                HttpStatus = StatusCodes.Status404NotFound,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Onbestaande gemeente.",
                ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext)
            };
    }
}
