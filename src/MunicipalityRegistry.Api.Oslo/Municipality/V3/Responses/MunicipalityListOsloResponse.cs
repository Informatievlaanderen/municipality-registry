namespace MunicipalityRegistry.Api.Oslo.Municipality.V3.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Oslo;
    using Be.Vlaanderen.Basisregisters.GrAr.Oslo.Gemeente;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    public class MunicipalityListOsloResponse
    {
        /// <summary>
        /// De linked-data context van de gemeente.
        /// </summary>
        [JsonProperty("@context", Required = Required.DisallowNull, Order = 0)]
        public required string Context { get; set; }

        /// <summary>
        /// Linked-data type.
        /// </summary>
        [JsonProperty("@type", Required = Required.DisallowNull, Order = 1)]
        public string Type => "DataEnvelop";

        /// <summary>
        /// De verzameling van gemeentes.
        /// </summary>
        [JsonProperty("data", Required = Required.DisallowNull, Order = 2)]
        public required List<MunicipalityListOsloItemResponse> Gemeenten { get; set; }

        /// <summary>
        /// De URL voor het ophalen van de volgende verzameling.
        /// </summary>
        [JsonProperty("volgende", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public Uri? Volgende { get; set; }
    }

    public class MunicipalityListOsloItemResponse
    {
        /// <summary>
        /// Het linked-data type van de gemeente.
        /// </summary>
        [JsonProperty("@type", Required = Required.DisallowNull, Order = 0)]
        public string Type => "Gemeente";

        /// <summary>
        /// De unieke en persistente identificator van de gemeente (volgt de Vlaamse URI-standaard).
        /// </summary>
        [JsonProperty("@id", Required = Required.DisallowNull, Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [JsonProperty("identificator", Required = Required.DisallowNull, Order = 2)]
        public GemeenteIdentificator Identificator { get; private set; }

        /// <summary>
        /// De URL die de details van de meest recente versie van de gemeente weergeeft.
        /// </summary>
        [JsonProperty("detail", Required = Required.DisallowNull, Order = 3)]
        public Uri Detail { get; private set; }

        /// <summary>
        /// De gemeentenaam in de officiële talen van de gemeente.
        /// </summary>
        [JsonProperty("naam", Required = Required.DisallowNull, Order = 4)]
        public Gemeentenaam Gemeentenaam { get; private set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [JsonProperty("status", Required = Required.DisallowNull, Order = 5)]
        public Status GemeenteStatus { get; private set; }

        public MunicipalityListOsloItemResponse(
            string nisCode,
            string detail,
            DateTimeOffset version,
            IEnumerable<GeografischeNaam> geografischeNamen,
            MunicipalityStatus? municipalityStatus)
        {
            Id = OsloNamespaces.Gemeente.ToPuri(nisCode);
            Identificator = new GemeenteIdentificator(nisCode, version);
            Detail = new Uri(string.Format(detail, nisCode));
            Gemeentenaam = new Gemeentenaam
            {
                Gemeentenamen = geografischeNamen.ToList()
            };
            GemeenteStatus = new Status(municipalityStatus.ConvertOsloFromMunicipalityStatus());
        }
    }

    public class MunicipalityListOsloResponseExamples : IExamplesProvider<MunicipalityListOsloResponse>
    {
        private readonly ResponseOptionsV3 _responseOptions;

        public MunicipalityListOsloResponseExamples(IOptions<ResponseOptionsV3> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public MunicipalityListOsloResponse GetExamples()
        {
            var municipalitySamples = new List<MunicipalityListOsloItemResponse>
            {
                new MunicipalityListOsloItemResponse(
                    "31005",
                    _responseOptions.DetailUrl,
                    DateTimeOffset.Now.ToExampleOffset(),
                    [new GeografischeNaam("Brugge", Taal.Nl)],
                    MunicipalityStatus.Current),

                new MunicipalityListOsloItemResponse(
                    "53084",
                    _responseOptions.DetailUrl,
                    DateTimeOffset.Now.AddHours(32).ToExampleOffset(),
                    [new GeografischeNaam("Quévy", Taal.Fr)],
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
