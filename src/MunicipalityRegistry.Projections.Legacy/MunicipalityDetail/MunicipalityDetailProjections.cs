namespace MunicipalityRegistry.Projections.Legacy.MunicipalityDetail
{
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using NodaTime;

    [ConnectedProjectionName("API endpoint detail gemeenten")]
    [ConnectedProjectionDescription("Projectie die de gemeenten data voor het gemeenten detail voorziet.")]
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
                            Status = MunicipalityStatus.Proposed,
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
                        municipality.AddOfficialLanguage(message.Message.Language);
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
                        municipality.RemoveOfficialLanguage(message.Message.Language);
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
                        municipality.AddFacilitiesLanguage(message.Message.Language);
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipalityDetail(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.RemoveFacilitiesLanguage(message.Message.Language);
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

            When<Envelope<MunicipalityWasMerged>>(async (context, message, ct) =>
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

            When<Envelope<MunicipalityGeometryWasCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrected>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityGeometryWasCorrectedToCleared>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasDrawn>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityNameWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
            When<Envelope<MunicipalityWasImportedFromCrab>>(async (context, message, ct) => await DoNothing());
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

        private static async Task DoNothing()
        {
            await Task.Yield();
        }
    }
}
