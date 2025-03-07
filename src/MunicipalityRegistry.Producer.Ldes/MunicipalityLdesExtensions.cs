namespace MunicipalityRegistry.Producer.Ldes
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class MunicipalityLdesExtensions
    {
        public static async Task<MunicipalityDetail> FindAndUpdateMunicipalityDetail(
            this ProducerContext context,
            Guid municipalityId,
            Action<MunicipalityDetail> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .Municipalities
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            return municipality;
        }

        private static ProjectionItemNotFoundException<ProducerProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<ProducerProjections>(municipalityId.ToString("D"));

        public static Taal ConvertFromLanguage(this Language? language)
            => ConvertFromLanguage(language ?? Language.Dutch);

        public static Taal ConvertFromLanguage(this Language language)
        {
            switch (language)
            {
                default:
                case Language.Dutch:
                    return Taal.NL;

                case Language.French:
                    return Taal.FR;

                case Language.German:
                    return Taal.DE;

                case Language.English:
                    return Taal.EN;
            }
        }

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
    }
}
