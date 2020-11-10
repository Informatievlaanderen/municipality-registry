namespace MunicipalityRegistry.Projections.Legacy.MunicipalityName
{
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using NodaTime;

    public class MunicipalityNameProjections : ConnectedProjection<LegacyContext>
    {
        public MunicipalityNameProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .MunicipalityName
                    .AddAsync(
                        new MunicipalityName
                        {
                            MunicipalityId = message.Message.MunicipalityId,
                            NisCode = message.Message.NisCode,
                            IsFlemishRegion = RegionFilter.IsFlemishRegion(message.Message.NisCode),
                            VersionTimestamp = message.Message.Provenance.Timestamp
                        }, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityName(
                    message.Message.MunicipalityId,
                    municipalityNameItem =>
                    {
                        municipalityNameItem.NisCode = message.Message.NisCode;
                        municipalityNameItem.IsFlemishRegion = RegionFilter.IsFlemishRegion(message.Message.NisCode);
                        UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityName(
                    message.Message.MunicipalityId,
                    municipalityNameItem =>
                    {
                        municipalityNameItem.NisCode = message.Message.NisCode;
                        municipalityNameItem.IsFlemishRegion = RegionFilter.IsFlemishRegion(message.Message.NisCode);
                        UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityName(
                    message.Message.MunicipalityId,
                    municipalityNameItem =>
                    {
                        UpdateNameByLanguage(municipalityNameItem, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityName(
                    message.Message.MunicipalityId,
                    municipalityNameItem =>
                    {
                        UpdateNameByLanguage(municipalityNameItem, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityName(
                    message.Message.MunicipalityId,
                    municipalityNameItem =>
                    {
                        UpdateNameByLanguage(municipalityNameItem, message.Message.Language, null);
                        UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityName(
                    message.Message.MunicipalityId,
                    municipalityNameItem =>
                    {
                        UpdateNameByLanguage(municipalityNameItem, message.Message.Language, null);
                        UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
        }

        private static void UpdateNameByLanguage(MunicipalityName municipalityName, Language language, string name)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipalityName.NameDutch = name;
                    municipalityName.NameDutchSearch = name?.SanitizeForBosaSearch();
                    break;

                case Language.French:
                    municipalityName.NameFrench = name;
                    municipalityName.NameFrenchSearch = name?.SanitizeForBosaSearch();
                    break;

                case Language.German:
                    municipalityName.NameGerman = name;
                    municipalityName.NameGermanSearch = name?.SanitizeForBosaSearch();
                    break;

                case Language.English:
                    municipalityName.NameEnglish = name;
                    municipalityName.NameEnglishSearch = name?.SanitizeForBosaSearch();
                    break;
            }
        }

        private static void UpdateVersionTimestamp(MunicipalityName municipalityNameItem, Instant versionTimestamp)
            => municipalityNameItem.VersionTimestamp = versionTimestamp;

        private static void DoNothing() { }
    }
}
