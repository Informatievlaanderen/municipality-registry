namespace MunicipalityRegistry.Projections.WmsWfs.MunicipalityDetail
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using Municipality.Events;
    using NodaTime;

    [ConnectedProjectionName("API endpoint detail gemeenten")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor het gemeenten detail voorziet.")]
    public class MunicipalityDetailProjections : ConnectedProjection<WmsWfsContext>
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

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilitiesLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
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
                        municipality.Status = MunicipalityStatus.Current.ToString();
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
                        municipality.Status = MunicipalityStatus.Current.ToString();
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
                        municipality.Status = MunicipalityStatus.Retired.ToString();
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
                        municipality.Status = MunicipalityStatus.Retired.ToString();
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
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

        private static void UpdateNameByLanguage(MunicipalityDetail municipality, Language? language, string name)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipality.NameDutch = name;
                    break;
            }
        }

        private static void UpdateVersionTimestamp(MunicipalityDetail municipality, Instant versionTimestamp)
            => municipality.VersionTimestamp = versionTimestamp;

        private static void DoNothing() { }
    }
}
