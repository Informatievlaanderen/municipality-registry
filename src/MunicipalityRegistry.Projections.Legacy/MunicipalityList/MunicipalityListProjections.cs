namespace MunicipalityRegistry.Projections.Legacy.MunicipalityList
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using NodaTime;

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
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
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
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
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
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
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
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.PrimaryLanguage = message.Message.Language;
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.PrimaryLanguage = message.Message.Language;
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.PrimaryLanguage = null;
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.PrimaryLanguage = null;
                        UpdateDefaultNameByPrimaryLanguage(municipalityListItem);
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.SecondaryLanguage = message.Message.Language;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.SecondaryLanguage = message.Message.Language;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.SecondaryLanguage = null;
                        UpdateVersionTimestamp(municipalityListItem, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityListItem(
                    message.Message.MunicipalityId,
                    municipalityListItem =>
                    {
                        municipalityListItem.SecondaryLanguage = null;
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

        private static void UpdateDefaultNameByPrimaryLanguage(MunicipalityListItem municipalityListItem)
        {
            switch (municipalityListItem.PrimaryLanguage)
            {
                case null:
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
    }
}
