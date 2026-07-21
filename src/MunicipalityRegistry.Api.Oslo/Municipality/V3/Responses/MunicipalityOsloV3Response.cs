namespace MunicipalityRegistry.Api.Oslo.Municipality.V3.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Oslo;
    using Be.Vlaanderen.Basisregisters.GrAr.Oslo.Gemeente;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    public class MunicipalityOsloV3Response
    {
        /// <summary>
        /// De linked-data context van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "@context", Order = 0, Required = Required.DisallowNull)]
        public string Context { get; }

        /// <summary>
        /// Het linked-data type van de envelop.
        /// </summary>
        [JsonProperty(PropertyName = "@type", Order = 1, Required = Required.DisallowNull)]
        public string Type => "DataEnvelop";

        /// <summary>
        /// De details van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "data", Order = 2, Required = Required.DisallowNull)]
        public MunicipalityDetailOsloV3ResponseData V3ResponseData { get; set; }

        /// <summary>
        /// De hyperlinks die gerelateerd zijn aan de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "_links", Order = 99, Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MunicipalityDetailOsloV3ResponseLinks? Links { get; set; }

        public MunicipalityOsloV3Response(
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
            V3ResponseData = new MunicipalityDetailOsloV3ResponseData(
                status,
                nisCode,
                officialLanguages,
                facilitiesLanguages,
                nameDutch,
                nameFrench,
                nameGerman,
                nameEnglish,
                version);

            Links = new MunicipalityDetailOsloV3ResponseLinks(
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

    public class MunicipalityDetailOsloV3ResponseData
    {
        /// <summary>
        /// Het linked-data type van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "@type", Order = 0, Required = Required.DisallowNull)]
        public string Type => "Gemeente";

        /// <summary>
        /// De unieke en persistente identificator van de gemeente (volgt de Vlaamse URI-standaard).
        /// </summary>
        [JsonProperty(PropertyName = "@id", Order = 1, Required = Required.DisallowNull)]
        public string Id { get; set; }

        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "identificator", Order = 2, Required = Required.DisallowNull)]
        public GemeenteIdentificator Identificator { get; private set; }

        /// <summary>
        /// De officiële talen van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "officieleTaal", Order = 3, Required = Required.DisallowNull)]
        public List<MunicipalityDetailOsloV3Language> OfficialLanguages { get; private set; }

        /// <summary>
        /// De faciliteiten talen van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "faciliteitenTaal", Order = 4, Required = Required.DisallowNull)]
        public List<MunicipalityDetailOsloV3Language> FacilitiesLanguages { get; private set; }

        /// <summary>
        /// De officiële namen van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "naam", Order = 5, Required = Required.DisallowNull)]
        public Gemeentenaam Gemeentenamen { get; private set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "status", Order = 6, Required = Required.DisallowNull)]
        public Status GemeenteStatus { get; private set; }

        public MunicipalityDetailOsloV3ResponseData(
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
            Id = OsloNamespaces.Gemeente.ToPuri(nisCode);
            Identificator = new GemeenteIdentificator(nisCode, version);
            GemeenteStatus = new Status(status);
            OfficialLanguages = officialLanguages
                .Select(LanguageExtensions.ConvertOsloFromLanguage)
                .Select(x => new MunicipalityDetailOsloV3Language(x)).ToList();
            FacilitiesLanguages = facilitiesLanguages
                .Select(LanguageExtensions.ConvertOsloFromLanguage)
                .Select(x => new MunicipalityDetailOsloV3Language(x)).ToList();

            var gemeenteNamen = new List<GeografischeNaam>
            {
                new GeografischeNaam(nameDutch, Taal.Nl),
                new GeografischeNaam(nameFrench, Taal.Fr),
                new GeografischeNaam(nameGerman, Taal.De),
                new GeografischeNaam(nameEnglish, Taal.En)
            };

            Gemeentenamen = new Gemeentenaam()
            {
                Gemeentenamen = gemeenteNamen.Where(x => !string.IsNullOrEmpty(x.Spelling)).ToList()
            };
        }
    }

    public class MunicipalityDetailOsloV3Language
    {
        /// <summary>
        /// Het linked-data type van de taal.
        /// </summary>
        [JsonProperty(PropertyName = "@type", Required = Required.DisallowNull, Order = 0)]
        public string Type => "Language";

        /// <summary>
        /// De taal van de gemeente.
        /// </summary>
        [JsonProperty(PropertyName = "@value", Required = Required.DisallowNull, Order = 1)]
        public Taal Value { get; set; }

        public MunicipalityDetailOsloV3Language(Taal taal)
        {
            Value = taal;
        }
    }

    /// <summary>
    /// De hyperlinks die gerelateerd zijn aan de gemeente.
    /// </summary>
    public class MunicipalityDetailOsloV3ResponseLinks
    {
        [JsonProperty(PropertyName = "@type", Required = Required.DisallowNull, Order = 0)]
        public string Type => "Links";

        [JsonProperty(PropertyName = "self", Required = Required.DisallowNull, Order = 1)]
        public Link Self { get; set; }

        [JsonProperty(PropertyName = "straatnamen", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 2)]
        public Link? Straatnamen { get; set; }

        [JsonProperty(PropertyName = "adressen", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 3)]
        public Link? Adressen { get; set; }

        [JsonProperty(PropertyName = "postinfo", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 4)]
        public Link? PostInfo { get; set; }

        public MunicipalityDetailOsloV3ResponseLinks(
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

    public class MunicipalityOsloResponseExamples : IExamplesProvider<MunicipalityOsloV3Response>
    {
        private readonly ResponseOptionsV3 _responseOptions;

        public MunicipalityOsloResponseExamples(IOptions<ResponseOptionsV3> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public MunicipalityOsloV3Response GetExamples()
            => new MunicipalityOsloV3Response(
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
                ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext, "v3")
            };
    }

    public class MunicipalityGoneResponseExamples : IExamplesProvider<ProblemDetails>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProblemDetailsHelper _problemDetailsHelper;

        public MunicipalityGoneResponseExamples(
            IHttpContextAccessor httpContextAccessor,
            ProblemDetailsHelper problemDetailsHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _problemDetailsHelper = problemDetailsHelper;
        }

        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                ProblemTypeUri = "urn:be.vlaanderen.basisregisters.api:municipality:gone",
                HttpStatus = StatusCodes.Status410Gone,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Verwijderde gemeente.",
                ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext, "v3")
            };
    }
}
