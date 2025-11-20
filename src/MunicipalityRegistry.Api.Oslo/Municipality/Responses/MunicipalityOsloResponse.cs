namespace MunicipalityRegistry.Api.Oslo.Municipality.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
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
        /// De linked-data context van de gemeente.
        /// </summary>
        [DataMember(Name = "@context", Order = 0)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Context { get; }

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

        /// <summary>
        /// De hyperlinks die gerelateerd zijn aan de gemeente.
        /// </summary>
        [DataMember(Name = "_links", Order = 99)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MunicipalityDetailOsloResponseLinks? Links { get; set; }

        public MunicipalityOsloResponse(
            string naamruimte,
            string contextUrlDetail,
            GemeenteStatus status,
            string nisCode,
            IEnumerable<Language> officialLanguages,
            IEnumerable<Language> facilitiesLanguages,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish,
            DateTimeOffset version,
            string selfDetailUrl,
            string straatnamenLinkUrl,
            string adressenLinkUrl,
            string postInfoLinkUrl)
        {
            Context = contextUrlDetail;
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

            Links = new MunicipalityDetailOsloResponseLinks(
                self: new Link
                {
                    Href = new Uri(string.Format(selfDetailUrl, nisCode))
                },
                straatnamen: new Link
                {
                    Href = new Uri(string.Format(straatnamenLinkUrl, nisCode))
                },
                adressen: new Link
                {
                    Href = new Uri(string.Format(adressenLinkUrl, nisCode))
                },
                postInfo: new Link
                {
                    Href = new Uri(string.Format(postInfoLinkUrl, nameDutch ?? nameFrench))
                }
            );
        }
    }

    [DataContract(Name = "_links", Namespace = "")]
    public class MunicipalityDetailOsloResponseLinks
    {
        [DataMember(Name = "self")]
        [JsonProperty(Required = Required.DisallowNull)]
        public Link Self { get; set; }

        [DataMember(Name = "straatnamen", EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Link? Straatnamen { get; set; }

        [DataMember(Name = "adressen", EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Link? Adressen { get; set; }

        [DataMember(Name = "postinfo", EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Link? PostInfo { get; set; }

        public MunicipalityDetailOsloResponseLinks(
            Link self,
            Link? straatnamen = null,
            Link? adressen = null,
            Link? postInfo = null)
        {
            Self = self;
            Straatnamen = straatnamen;
            Adressen = adressen;
            PostInfo = postInfo;
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
                _responseOptions.ContextUrlDetail,
                GemeenteStatus.InGebruik,
                "31005",
                new List<Language> { Language.Dutch },
                new List<Language> { Language.French },
                "Brugge",
                "Bruges",
                "Brügge",
                "Bruges",
                DateTimeOffset.Now.ToExampleOffset(),
                _responseOptions.DetailUrl,
                _responseOptions.MunicipalityDetailStreetNamesLink,
                _responseOptions.MunicipalityDetailAddressesLink,
                _responseOptions.MunicipalityDetailPostInfoLink);
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
                ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext, "v2")
            };
    }
}
