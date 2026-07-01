namespace MunicipalityRegistry.Api.Oslo.Municipality.V3.Responses
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Swashbuckle.AspNetCore.Filters;

    public class TotalCountResponseExample : IExamplesProvider<TotaalAantalResponse>
    {
        public TotaalAantalResponse GetExamples()
        {
            return new TotaalAantalResponse
            {
                Aantal = 574512
            };
        }
    }
}
