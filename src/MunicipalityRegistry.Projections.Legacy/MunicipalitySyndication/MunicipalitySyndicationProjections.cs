namespace MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication
{
    using System;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;

    [ConnectedProjectionName("Legacy - MunicipalitySyndication")]
    [ConnectedProjectionDescription("Gemeente data voor de feed.")]
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
                    ChangeType = message.EventName,
                    SyndicationItemCreatedAt = DateTimeOffset.UtcNow
                };

                newMunicipalitySyndicationItem.ApplyProvenance(message.Message.Provenance);
                newMunicipalitySyndicationItem.SetEventData(message.Message, message.EventName);

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
                        UpdateDefaultName(x);
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
                        UpdateDefaultName(x);
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
                        UpdateDefaultName(x);
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
                        UpdateDefaultName(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.AddOfficialLanguage(message.Message.Language);
                        UpdateDefaultName(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.RemoveOfficialLanguage(message.Message.Language);
                        UpdateDefaultName(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.AddFacilitiesLanguage(message.Message.Language);
                        UpdateDefaultName(x);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalitySyndicationItem(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.RemoveFacilitiesLanguage(message.Message.Language);
                        UpdateDefaultName(x);
                    },
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

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
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

        private static void UpdateDefaultName(MunicipalitySyndicationItem municipalitySyndicationItem)
        {
            switch (municipalitySyndicationItem.OfficialLanguages.FirstOrDefault())
            {
                default:
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

        private static void DoNothing() { }
    }
}
