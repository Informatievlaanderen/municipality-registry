namespace MunicipalityRegistry.Api.Legacy.Municipality.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Infrastructure.Options;
    using Microsoft.Extensions.Options;
    using Requests;
    using Swashbuckle.AspNetCore.Filters;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Bosa;

    [DataContract(Name = "GemeenteCollectie", Namespace = "")]
    public class MunicipalityBosaResponse
    {
        /// <summary>
        /// De verzameling van gemeentes.
        /// </summary>
        [DataMember(Name = "Gemeenten", Order = 1)]
        public List<MunicipalityBosaItemResponse> Gemeenten { get; set; }

        /// <summary>
        /// Het totaal aantal gemeenten die overeenkomen met de vraag.
        /// </summary>
        [DataMember(Name = "TotaalAantal", Order = 2)]
        public long TotaalAantal { get; set; }
    }

    [DataContract(Name = "GemeenteCollectieItem", Namespace = "")]
    public class MunicipalityBosaItemResponse
    {
        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [DataMember(Name = "Identificator", Order = 1)]
        public Identificator Identificator { get; set; }

        /// <summary>
        /// Een lijst van namen van de gemeente, per taal.
        /// </summary>
        [DataMember(Name = "Gemeentenamen", Order = 2)]
        public List<Gemeentenaam> Gemeentenamen { get; set; }

        public MunicipalityBosaItemResponse(string id, string naamruimte, DateTimeOffset version, IEnumerable<GeografischeNaam> geografischeNamen)
        {
            Identificator = new Identificator(naamruimte, id, version);
            Gemeentenamen = geografischeNamen.Select(g => new Gemeentenaam(g)).ToList();
        }
    }

    public class MunicipalityBosaResponseExamples : IExamplesProvider
    {
        private readonly ResponseOptions _responseOptions;

        public MunicipalityBosaResponseExamples(IOptions<ResponseOptions> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public object GetExamples()
        {
            var municipalityExamples = new List<MunicipalityBosaItemResponse>
            {
                new MunicipalityBosaItemResponse("13014", _responseOptions.Naamruimte, DateTimeOffset.Now, new []
                {
                    new GeografischeNaam("Hoogstraten", Taal.NL)
                }),
                new MunicipalityBosaItemResponse("13035", _responseOptions.Naamruimte, DateTimeOffset.Now.AddHours(32), new []
                {
                    new GeografischeNaam("Ravels", Taal.NL)
                })
            };

            return new MunicipalityBosaResponse
            {
                Gemeenten = municipalityExamples,
                TotaalAantal = 2
            };
        }
    }

    public class MunicipalityBosaRequestExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new BosaMunicipalityRequest
            {
                Gemeentenaam = new ZoekGeografischeNaam
                {
                    Taal = Taal.NL,
                    Spelling = "ra",
                    SearchType = BosaSearchType.Bevat
                },
                GemeenteCode = new ZoekIdentifier
                {
                    ObjectId = "13"
                }
            };
        }
    }
}
