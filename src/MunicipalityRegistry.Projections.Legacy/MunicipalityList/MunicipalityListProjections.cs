namespace MunicipalityRegistry.Projections.Legacy.MunicipalityList
{
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using NodaTime;

    [ConnectedProjectionName("API endpoint lijst gemeenten")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor de gemeenten lijst voorziet.")]
    public class MunicipalityListProjections : ConnectedProjection<LegacyContext>
    {
        public MunicipalityListProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .MunicipalityList
                    .AddAsync(
                        new MunicipalityListItem
                        {
                            MunicipalityId = message.Message.MunicipalityId,
                            NisCode = message.Message.NisCode,
                            VersionTimestamp = message.Message.Provenance.Timestamp
                        }, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.NisCode = message.Message.NisCode;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.NisCode = message.Message.NisCode;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        UpdateNameByLanguage(municipalityListItem, message.Message.Language, message.Message.Name);
                        UpdateDefaultNameByOfficialLanguages(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        UpdateNameByLanguage(municipalityListItem, message.Message.Language, message.Message.Name);
                        UpdateDefaultNameByOfficialLanguages(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        UpdateNameByLanguage(municipalityListItem, message.Message.Language, null);
                        UpdateDefaultNameByOfficialLanguages(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        UpdateNameByLanguage(municipalityListItem, message.Message.Language, null);
                        UpdateDefaultNameByOfficialLanguages(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.AddOfficialLanguage(message.Message.Language);
                        UpdateDefaultNameByOfficialLanguages(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.RemoveOfficialLanguage(message.Message.Language);
                        UpdateDefaultNameByOfficialLanguages(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.Status = MunicipalityStatus.Current;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.Status = MunicipalityStatus.Current;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.Status = MunicipalityStatus.Retired;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.Status = MunicipalityStatus.Retired;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
        }

        private static void UpdateNameByLanguage(MunicipalityListItem municipalityListItem, Language? language, string name)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipalityListItem.NameDutch = name;
                    break;

                case Language.French:
                    municipalityListItem.NameFrench = name;
                    break;

                case Language.German:
                    municipalityListItem.NameGerman = name;
                    break;

                case Language.English:
                    municipalityListItem.NameEnglish = name;
                    break;
            }
        }

        private static void UpdateDefaultNameByOfficialLanguages(MunicipalityListItem municipalityListItem)
        {
            switch (municipalityListItem.OfficialLanguages.FirstOrDefault())
            {
                case Language.Dutch:
                    municipalityListItem.DefaultName = municipalityListItem.NameDutch;
                    break;

                case Language.French:
                    municipalityListItem.DefaultName = municipalityListItem.NameFrench;
                    break;

                case Language.German:
                    municipalityListItem.DefaultName = municipalityListItem.NameGerman;
                    break;

                case Language.English:
                    municipalityListItem.DefaultName = municipalityListItem.NameEnglish;
                    break;
            }
        }

        private static void UpdateVersionTimestamp(MunicipalityListItem municipalityListItem, Instant timestamp)
            => municipalityListItem.VersionTimestamp = timestamp;

        private static void DoNothing() { }
    }
}
