namespace MunicipalityRegistry.Projections.Legacy.MunicipalityDetail
{
    using System;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using NodaTime;

    public class MunicipalityDetailProjections : ConnectedProjection<LegacyContext>
    {
        public MunicipalityDetailProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .MunicipalityDetail
                    .AddAsync(
                        new MunicipalityDetail
                        {
                            MunicipalityId = message.Message.MunicipalityId,
                            NisCode = message.Message.NisCode,
                            VersionTimestamp = message.Message.Provenance.Timestamp
                        }, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.NisCode = message.Message.NisCode;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.NisCode = message.Message.NisCode;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipality, message.Message.Language, message.Message.Name);
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipality, message.Message.Language, message.Message.Name);
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipality, message.Message.Language, null);
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                UpdateNameByLanguage(municipality, message.Message.Language, null);
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.PrimaryLanguage = message.Message.Language;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.PrimaryLanguage = message.Message.Language;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.PrimaryLanguage = null;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.PrimaryLanguage = null;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.SecondaryLanguage = message.Message.Language;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.SecondaryLanguage = message.Message.Language;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.SecondaryLanguage = null;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.SecondaryLanguage = null;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.Status = MunicipalityStatus.Current;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.Status = MunicipalityStatus.Current;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.Status = MunicipalityStatus.Retired;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                var municipality = await context
                    .MunicipalityDetail
                    .FindAsync(message.Message.MunicipalityId, cancellationToken: ct);

                if (municipality == null)
                    throw DatabaseItemNotFound(message.Message.MunicipalityId);

                municipality.Status = MunicipalityStatus.Retired;
                UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
            });
        }

        private static void UpdateNameByLanguage(MunicipalityDetail municipality, Language? language, string name)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipality.NameDutch = name;
                    break;

                case Language.French:
                    municipality.NameFrench = name;
                    break;

                case Language.German:
                    municipality.NameGerman = name;
                    break;

                case Language.English:
                    municipality.NameEnglish = name;
                    break;
            }
        }

        private static void UpdateVersionTimestamp(MunicipalityDetail municipality, Instant versionTimestamp)
            => municipality.VersionTimestamp = versionTimestamp;

        private static ProjectionItemNotFoundException<MunicipalityDetailProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityDetailProjections>(municipalityId.ToString("D"));
    }
}
