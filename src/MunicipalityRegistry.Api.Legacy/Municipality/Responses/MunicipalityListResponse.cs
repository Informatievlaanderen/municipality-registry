namespace MunicipalityRegistry.Api.Legacy.Municipality.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.Filters;

    [DataContract(Name = "GemeenteCollectie", Namespace = "")]
    public class MunicipalityListResponse
    {
        /// <summary>
        /// De verzameling van gemeentes.
        /// </summary>
        [DataMember(Name = "Gemeenten", Order = 1)]
        public List<MunicipalityListItemResponse> Gemeenten { get; set; }

        /// <summary>
        /// Het totaal aantal gemeenten die overeenkomen met de vraag.
        /// </summary>
        [DataMember(Name = "TotaalAantal", Order = 2)]
        public long TotaalAantal { get; set; }

        /// <summary>
        /// De URL voor het ophalen van de volgende verzameling.
        /// </summary>
        [DataMember(Name = "Volgende", Order = 3, EmitDefaultValue = false)]
        public Uri Volgende { get; set; }
    }

    [DataContract(Name = "GemeenteCollectieItem", Namespace = "")]
    public class MunicipalityListItemResponse
    {
        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [DataMember(Name = "Identificator", Order = 1)]
        public Identificator Identificator { get; set; }

        /// <summary>
        /// De URL die naar de details van de meest recente versie van een enkele gemeente leidt.
        /// </summary>
        [DataMember(Name = "Detail", Order = 2)]
        public Uri Detail { get; set; }

        /// <summary>
        /// De naam van de gemeente in het Nederlands.
        /// </summary>
        [DataMember(Name = "Gemeentenaam", Order = 3)]
        public Gemeentenaam Gemeentenaam { get; set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [DataMember(Name = "GemeenteStatus", Order = 4)]
        public GemeenteStatus? GemeenteStatus { get; set; }

        public MunicipalityListItemResponse(
            string id,
            string naamruimte,
            string detail,
            DateTimeOffset version,
            GeografischeNaam geografischeNaam,
            MunicipalityStatus? municipalityStatus)
        {
            Identificator = new Identificator(naamruimte, id, version);
            Detail = new Uri(string.Format(detail, id));
            Gemeentenaam = new Gemeentenaam(geografischeNaam);
            GemeenteStatus = municipalityStatus?.ConvertFromMunicipalityStatus();
        }
    }

    public class MunicipalityListResponseExamples : IExamplesProvider
    {
        private readonly ResponseOptions _responseOptions;

        public MunicipalityListResponseExamples(IOptions<ResponseOptions> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public object GetExamples()
        {
            var municipalitySamples = new List<MunicipalityListItemResponse>
            {
                new MunicipalityListItemResponse(
                    "31005",
                    _responseOptions.Naamruimte,
                    _responseOptions.DetailUrl,
                    DateTimeOffset.Now,
                    new GeografischeNaam("Brugge", Taal.NL),
                    MunicipalityStatus.Current),

                new MunicipalityListItemResponse(
                    "53084",
                    _responseOptions.Naamruimte,
                    _responseOptions.DetailUrl,
                    DateTimeOffset.Now.AddHours(32),
                    new GeografischeNaam("Quévy", Taal.FR),
                    MunicipalityStatus.Retired)
            };

            return new MunicipalityListResponse
            {
                Gemeenten = municipalitySamples,
                TotaalAantal = 2,
                Volgende = new Uri(string.Format(_responseOptions.VolgendeUrl, 2, 10))
            };
        }
    }
}
