namespace MunicipalityRegistry.Api.Legacy.Municipality.Responses
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.Filters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Name = "GemeenteDetail", Namespace = "")]
    public class MunicipalityResponse
    {
        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [DataMember(Name = "Identificator", Order = 1)]
        public Identificator Identificator { get; set; }

        /// <summary>
        /// De officiële talen van de gemeente.
        /// </summary>
        [DataMember(Name = "OfficieleTalen", Order = 2)]
        public List<Taal> OfficialLanguages { get; set; }

        /// <summary>
        /// De faciliteiten talen van de gemeente.
        /// </summary>
        [DataMember(Name = "FaciliteitenTalen", Order = 3)]
        public List<Taal> FacilitiesLanguages { get; set; }

        /// <summary>
        /// De officiële namen van de gemeente.
        /// </summary>
        [DataMember(Name = "Gemeentenamen", Order = 4)]
        public List<GeografischeNaam> Gemeentenamen { get; set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [DataMember(Name = "GemeenteStatus", Order = 5)]
        public GemeenteStatus GemeenteStatus { get; set; }

        public MunicipalityResponse(
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
            Identificator = new Identificator(naamruimte, nisCode, version);
            GemeenteStatus = status;
            OfficialLanguages = officialLanguages.Select(x => x.ConvertFromLanguage()).ToList();
            FacilitiesLanguages = facilitiesLanguages.Select(x => x.ConvertFromLanguage()).ToList();

            var gemeenteNamen = new List<GeografischeNaam>
            {
                new GeografischeNaam(nameDutch, Taal.NL),
                new GeografischeNaam(nameFrench, Taal.FR),
                new GeografischeNaam(nameGerman, Taal.DE),
                new GeografischeNaam(nameEnglish, Taal.EN),
            };

            Gemeentenamen = gemeenteNamen.Where(x => !string.IsNullOrEmpty(x.Spelling)).ToList();
        }
    }

    public class MunicipalityResponseExamples : IExamplesProvider
    {
        private readonly ResponseOptions _responseOptions;

        public MunicipalityResponseExamples(IOptions<ResponseOptions> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public object GetExamples()
            => new MunicipalityResponse(
                _responseOptions.Naamruimte,
                GemeenteStatus.InGebruik,
                "31005",
                new List<Language> { Language.Dutch },
                new List<Language> { Language.French },
                "Brugge",
                "Bruges",
                "Brügge",
                "Bruges",
                DateTimeOffset.Now);
    }

    public class MunicipalityNotFoundResponseExamples : IExamplesProvider
    {
        public object GetExamples()
            => new BasicApiProblem
            {
                HttpStatus = StatusCodes.Status404NotFound,
                Title = BasicApiProblem.DefaultTitle,
                Detail = "Onbestaande gemeente.",
                ProblemInstanceUri = BasicApiProblem.GetProblemNumber()
            };
    }
}
