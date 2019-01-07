namespace MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;

    public class MunicipalitySyndicationProjections : ConnectedProjection<LegacyContext>
    {
        public MunicipalitySyndicationProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                var newMunicipalitySyndicationItem = new MunicipalitySyndicationItem
                {
                    Position = message.Position,
                    MunicipalityId = message.Message.MunicipalityId,
                    NisCode = message.Message.NisCode,
                    RecordCreatedAt = message.Message.Provenance.Timestamp,
                    LastChangedOn = message.Message.Provenance.Timestamp,
                    ChangeType = message.EventName
                };

                newMunicipalitySyndicationItem.ApplyProvenance(message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.NisCode = message.Message.NisCode,
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.NisCode = message.Message.NisCode,
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, message.Message.Name);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, message.Message.Name);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, null);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, null);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.PrimaryLanguage = message.Message.Language;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.PrimaryLanguage = message.Message.Language;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.PrimaryLanguage = null;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.PrimaryLanguage = null;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.SecondaryLanguage = message.Message.Language,
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.SecondaryLanguage = message.Message.Language,
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.SecondaryLanguage = null,
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.SecondaryLanguage = null,
                    ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.Status = MunicipalityStatus.Current,
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.Status = MunicipalityStatus.Current,
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.Status = MunicipalityStatus.Retired,
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x => x.Status = MunicipalityStatus.Retired,
                    ct);
            });
        }

        private static void UpdateNameByLanguage(MunicipalitySyndicationItem municipalitySyndicationItem, Language? language, string name)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipalitySyndicationItem.NameDutch = name;
                    break;

                case Language.French:
                    municipalitySyndicationItem.NameFrench = name;
                    break;

                case Language.German:
                    municipalitySyndicationItem.NameGerman = name;
                    break;

                case Language.English:
                    municipalitySyndicationItem.NameEnglish = name;
                    break;
            }
        }

        private static void UpdateDefaultNameByPrimaryLanguage(MunicipalitySyndicationItem municipalitySyndicationItem)
        {
            switch (municipalitySyndicationItem.PrimaryLanguage)
            {
                default:
                case null:
                case Language.Dutch:
                    municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameDutch;
                    break;

                case Language.French:
                    municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameFrench;
                    break;

                case Language.German:
                    municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameGerman;
                    break;

                case Language.English:
                    municipalitySyndicationItem.DefaultName = municipalitySyndicationItem.NameEnglish;
                    break;
            }
        }
    }
}
