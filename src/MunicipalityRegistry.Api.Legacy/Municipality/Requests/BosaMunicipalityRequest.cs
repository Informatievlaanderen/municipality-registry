using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Bosa;

namespace MunicipalityRegistry.Api.Legacy.Municipality.Requests
{
    public class BosaMunicipalityRequest
    {
        public ZoekGeografischeNaam Gemeentenaam { get; set; }
        public ZoekIdentifier GemeenteCode { get; set; }
    }
}
