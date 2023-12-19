namespace MunicipalityRegistry.Projections.Integration.Convertors
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;

    public static class MunicipalityStatusExtensions
    {
        public static string ConvertFromMunicipalityStatus(this MunicipalityStatus status)
        {
            switch (status)
            {
                case MunicipalityStatus.Retired:
                    return GemeenteStatus.Gehistoreerd.ToString();

                case MunicipalityStatus.Proposed:
                    return GemeenteStatus.Voorgesteld.ToString();

                default:
                case MunicipalityStatus.Current:
                    return GemeenteStatus.InGebruik.ToString();
            }
        }
    }
}
