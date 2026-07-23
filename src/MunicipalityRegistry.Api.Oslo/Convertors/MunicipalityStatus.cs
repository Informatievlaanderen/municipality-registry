namespace MunicipalityRegistry.Api.Oslo.Convertors
{

    public static class MunicipalityStatusExtensions
    {
        public static Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus ConvertFromMunicipalityStatus(this MunicipalityStatus? status)
            => ConvertFromMunicipalityStatus(status ?? MunicipalityStatus.Current);

        public static Be.Vlaanderen.Basisregisters.GrAr.Oslo.Gemeente.GemeenteStatusValue ConvertOsloFromMunicipalityStatus(this MunicipalityStatus? status)
            => ConvertOsloFromMunicipalityStatus(status ?? MunicipalityStatus.Current);

        public static Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus ConvertFromMunicipalityStatus(this MunicipalityStatus status)
        {
            switch (status)
            {
                case MunicipalityStatus.Retired:
                    return Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus.Gehistoreerd;

                case MunicipalityStatus.Proposed:
                    return Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus.Voorgesteld;

                default:
                case MunicipalityStatus.Current:
                    return Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus.InGebruik;
            }
        }

        public static Be.Vlaanderen.Basisregisters.GrAr.Oslo.Gemeente.GemeenteStatusValue ConvertOsloFromMunicipalityStatus(this MunicipalityStatus status)
        {
            switch (status)
            {
                case MunicipalityStatus.Retired:
                    return Be.Vlaanderen.Basisregisters.GrAr.Oslo.Gemeente.GemeenteStatusValue.Gehistoreerd;

                case MunicipalityStatus.Proposed:
                    return Be.Vlaanderen.Basisregisters.GrAr.Oslo.Gemeente.GemeenteStatusValue.Voorgesteld;

                default:
                case MunicipalityStatus.Current:
                    return Be.Vlaanderen.Basisregisters.GrAr.Oslo.Gemeente.GemeenteStatusValue.InGebruik;
            }
        }

        public static MunicipalityStatus ConvertFromGemeenteStatus(this Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus status)
        {
            switch (status)
            {
                case Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus.Gehistoreerd:
                    return MunicipalityStatus.Retired;

                case Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus.Voorgesteld:
                    return MunicipalityStatus.Proposed;

                default:
                case Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente.GemeenteStatus.InGebruik:
                    return MunicipalityStatus.Current;
            }
        }
    }
}
