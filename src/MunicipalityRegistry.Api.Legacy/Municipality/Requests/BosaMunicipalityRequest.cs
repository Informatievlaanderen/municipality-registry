using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Bosa;

namespace MunicipalityRegistry.Api.Legacy.Municipality.Requests
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Swashbuckle.AspNetCore.Filters;

    public class BosaMunicipalityRequest
    {
        public ZoekGeografischeNaam Gemeentenaam { get; set; }
        public ZoekIdentifier GemeenteCode { get; set; }
    }

    public class MunicipalityBosaRequestExamples : IExamplesProvider
    {
        public object GetExamples()
            => new BosaMunicipalityRequest
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
