namespace MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;

    // TODO: Make MunicipalityVersionProjections and MunicipalitySyndicationProjections more consistent
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

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.NisCode = message.Message.NisCode);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.NisCode = message.Message.NisCode);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, message.Message.Name);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, message.Message.Name);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, null);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, null);
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        x.PrimaryLanguage = message.Message.Language;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        x.PrimaryLanguage = message.Message.Language;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        x.PrimaryLanguage = null;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x =>
                    {
                        x.PrimaryLanguage = null;
                        UpdateDefaultNameByPrimaryLanguage(x);
                    });

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.SecondaryLanguage = message.Message.Language);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.SecondaryLanguage = message.Message.Language);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.SecondaryLanguage = null);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.SecondaryLanguage = null);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.Status = MunicipalityStatus.Current);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.Status = MunicipalityStatus.Current);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.Status = MunicipalityStatus.Retired);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                var municipalitySyndicationItem = await context.LatestPosition(message.Message.MunicipalityId, ct);

                if (municipalitySyndicationItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                var newMunicipalitySyndicationItem = municipalitySyndicationItem.CloneAndApplyEventInfo(
                    message.Position,
                    message.EventName,
                    message.Message.Provenance.Timestamp,
                    x => x.Status = MunicipalityStatus.Retired);

                ApplyProvenance(newMunicipalitySyndicationItem, message.Message.Provenance);

                await context
                    .MunicipalitySyndication
                    .AddAsync(newMunicipalitySyndicationItem, ct);
            });
        }

        private static void ApplyProvenance(MunicipalitySyndicationItem item, ProvenanceData provenance)
        {
            item.Application = provenance.Application;
            item.Modification = provenance.Modification;
            item.Operator = provenance.Operator;
            item.Organisation = provenance.Organisation;
            item.Plan = provenance.Plan;
        }

        private static ProjectionItemNotFoundException<MunicipalitySyndicationProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalitySyndicationProjections>(municipalityId.ToString("D"));

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
