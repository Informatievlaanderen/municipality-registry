namespace MunicipalityRegistry.Api.Oslo.Convertors
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;

    public static class MunicipalityStatusExtensions
    {
        public static GemeenteStatus ConvertFromMunicipalityStatus(this MunicipalityStatus? status)
            => ConvertFromMunicipalityStatus(status ?? MunicipalityStatus.Current);

        public static GemeenteStatus ConvertFromMunicipalityStatus(this MunicipalityStatus status)
        {
            switch (status)
            {
                case MunicipalityStatus.Retired:
                    return GemeenteStatus.Gehistoreerd;

                case MunicipalityStatus.Proposed:
                    return GemeenteStatus.Voorgesteld;

                default:
                case MunicipalityStatus.Current:
                    return GemeenteStatus.InGebruik;
            }
        }

        public static MunicipalityStatus ConvertFromGemeenteStatus(this GemeenteStatus status)
        {
            switch (status)
            {
                case GemeenteStatus.Gehistoreerd:
                    return MunicipalityStatus.Retired;

                case GemeenteStatus.Voorgesteld:
                    return MunicipalityStatus.Proposed;

                default:
                case GemeenteStatus.InGebruik:
                    return MunicipalityStatus.Current;
            }
        }
    }
}
