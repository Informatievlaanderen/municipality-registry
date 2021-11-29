namespace MunicipalityRegistry.Api.Oslo.Municipality.Requests
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Bosa;
    using Swashbuckle.AspNetCore.Filters;

    public class BosaMunicipalityRequest
    {
        public ZoekGeografischeNaam Gemeentenaam { get; set; }
        public ZoekIdentifier GemeenteCode { get; set; }
    }

    public class MunicipalityBosaRequestExamples : IExamplesProvider<BosaMunicipalityRequest>
    {
        public BosaMunicipalityRequest GetExamples()
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
