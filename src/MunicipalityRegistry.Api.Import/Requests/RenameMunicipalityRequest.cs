namespace MunicipalityRegistry.Api.Import.Requests
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;

    public sealed class RenameMunicipalityRequest
    {
        public Taal Taal { get; set; }
        public string Naam { get; set; }
    }
}
