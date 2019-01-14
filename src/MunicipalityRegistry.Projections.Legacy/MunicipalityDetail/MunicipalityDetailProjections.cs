namespace MunicipalityRegistry.Projections.Legacy.MunicipalityDetail
{
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
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.NisCode = message.Message.NisCode;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.NisCode = message.Message.NisCode;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, null);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateNameByLanguage(municipality, message.Message.Language, null);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.PrimaryLanguage = message.Message.Language;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.PrimaryLanguage = message.Message.Language;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.PrimaryLanguage = null;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityPrimaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.PrimaryLanguage = null;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.SecondaryLanguage = message.Message.Language;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.SecondaryLanguage = message.Message.Language;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.SecondaryLanguage = null;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalitySecondaryLanguageWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.SecondaryLanguage = null;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Current;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Current;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
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
    }
}
