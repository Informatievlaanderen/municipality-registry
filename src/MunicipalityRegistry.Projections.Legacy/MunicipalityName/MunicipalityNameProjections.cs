namespace MunicipalityRegistry.Projections.Legacy.MunicipalityName
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using MunicipalityList;
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
                var municipalityNameItem = await context
                    .MunicipalityName
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipalityNameItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipalityNameItem.NisCode = message.Message.NisCode;
                municipalityNameItem.IsFlemishRegion = RegionFilter.IsFlemishRegion(message.Message.NisCode);
                UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                var municipalityNameItem = await context
                    .MunicipalityName
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipalityNameItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipalityNameItem.NisCode = message.Message.NisCode;
                municipalityNameItem.IsFlemishRegion = RegionFilter.IsFlemishRegion(message.Message.NisCode);
                UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                var municipalityNameItem = await context
                    .MunicipalityName
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipalityNameItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipalityNameItem, message.Message.Language, message.Message.Name);
                UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                var municipalityNameItem = await context
                    .MunicipalityName
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipalityNameItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipalityNameItem, message.Message.Language, message.Message.Name);
                UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                var municipalityNameItem = await context
                    .MunicipalityName
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipalityNameItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipalityNameItem, message.Message.Language, null);
                UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var municipalityNameItem = await context
                    .MunicipalityName
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipalityNameItem == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipalityNameItem, message.Message.Language, null);
                UpdateVersionTimestamp(municipalityNameItem, message.Message.Provenance.Timestamp);
            });
        }

        private static void UpdateVersionTimestamp(MunicipalityName municipalityNameItem, Instant versionTimestamp)
        {
            municipalityNameItem.VersionTimestamp = versionTimestamp;
        }

        private static ProjectionItemNotFoundException<MunicipalityListProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityListProjections>(municipalityId.ToString("D"));

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
    }
}
