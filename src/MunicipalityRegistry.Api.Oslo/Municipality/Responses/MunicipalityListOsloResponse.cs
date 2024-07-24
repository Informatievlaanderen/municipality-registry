namespace MunicipalityRegistry.Api.Oslo.Municipality.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    [DataContract(Name = "GemeenteCollectie", Namespace = "")]
    public class MunicipalityListOsloResponse
    {
        /// <summary>
        /// De linked-data context van de gemeente.
        /// </summary>
        [DataMember(Name = "@context", Order = 0)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Context { get; set; }

        /// <summary>
        /// De verzameling van gemeentes.
        /// </summary>
        [DataMember(Name = "Gemeenten", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<MunicipalityListOsloItemResponse> Gemeenten { get; set; }

        /// <summary>
        /// De URL voor het ophalen van de volgende verzameling.
        /// </summary>
        [DataMember(Name = "Volgende", Order = 3, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri Volgende { get; set; }
    }

    [DataContract(Name = "GemeenteCollectieItem", Namespace = "")]
    public class MunicipalityListOsloItemResponse
    {
        /// <summary>
        /// Het linked-data type van de gemeente.
        /// </summary>
        [DataMember(Name = "@type", Order = 0)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Type => "Gemeente";

        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [DataMember(Name = "Identificator", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public GemeenteIdentificator Identificator { get; private set; }

        /// <summary>
        /// De URL die de details van de meest recente versie van de gemeente weergeeft.
        /// </summary>
        [DataMember(Name = "Detail", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Uri Detail { get; private set; }

        /// <summary>
        /// De gemeentenaam in de eerste officiële taal van de gemeente.
        /// </summary>
        [DataMember(Name = "Gemeentenaam", Order = 3)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Gemeentenaam Gemeentenaam { get; private set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [DataMember(Name = "GemeenteStatus", Order = 4)]
        [JsonProperty(Required = Required.DisallowNull)]
        public GemeenteStatus GemeenteStatus { get; private set; }

        public MunicipalityListOsloItemResponse(
            string id,
            string naamruimte,
            string detail,
            DateTimeOffset version,
            GeografischeNaam geografischeNaam,
            MunicipalityStatus? municipalityStatus)
        {
            Identificator = new GemeenteIdentificator(naamruimte, id, version);
            Detail = new Uri(string.Format(detail, id));
            Gemeentenaam = new Gemeentenaam(geografischeNaam);
            GemeenteStatus = municipalityStatus.ConvertFromMunicipalityStatus();
        }
    }

    public class MunicipalityListOsloResponseExamples : IExamplesProvider<MunicipalityListOsloResponse>
    {
        private readonly ResponseOptions _responseOptions;

        public MunicipalityListOsloResponseExamples(IOptions<ResponseOptions> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public MunicipalityListOsloResponse GetExamples()
        {
            var municipalitySamples = new List<MunicipalityListOsloItemResponse>
            {
                new MunicipalityListOsloItemResponse(
                    "31005",
                    _responseOptions.Naamruimte,
                    _responseOptions.DetailUrl,
                    DateTimeOffset.Now.ToExampleOffset(),
                    new GeografischeNaam("Brugge", Taal.NL),
                    MunicipalityStatus.Current),

                new MunicipalityListOsloItemResponse(
                    "53084",
                    _responseOptions.Naamruimte,
                    _responseOptions.DetailUrl,
                    DateTimeOffset.Now.AddHours(32).ToExampleOffset(),
                    new GeografischeNaam("Quévy", Taal.FR),
                    MunicipalityStatus.Retired)
            };

            return new MunicipalityListOsloResponse
            {
                Context = _responseOptions.ContextUrlList,
                Gemeenten = municipalitySamples,
                Volgende = new Uri(string.Format(_responseOptions.VolgendeUrl, 2, 10))
            };
        }
    }
}
