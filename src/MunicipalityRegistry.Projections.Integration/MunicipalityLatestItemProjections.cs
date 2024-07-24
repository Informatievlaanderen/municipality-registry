namespace MunicipalityRegistry.Projections.Integration
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Convertors;
    using Infrastructure;
    using Microsoft.Extensions.Options;
    using Municipality.Events;
    using NodaTime;

    [ConnectedProjectionName("Integratie gemeente latest item")]
    [ConnectedProjectionDescription("Projectie die de laatste gemeente data voor de integratie database bijhoudt.")]
    public sealed class MunicipalityLatestItemProjections : ConnectedProjection<IntegrationContext>
    {
        public MunicipalityLatestItemProjections(IOptions<IntegrationOptions> options)
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .MunicipalityLatestItems
                    .AddAsync(
                        new MunicipalityLatestItem
                        {
                            MunicipalityId = message.Message.MunicipalityId,
                            NisCode = message.Message.NisCode,
                            VersionTimestamp = message.Message.Provenance.Timestamp,
                            Namespace = options.Value.Namespace,
                            PuriId = $"{options.Value.Namespace}/{message.Message.NisCode}",
                            Status = MunicipalityStatus.Proposed
                        }, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.NisCode = message.Message.NisCode;
                        municipality.PuriId = $"{options.Value.Namespace}/{message.Message.NisCode}";
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.NisCode = message.Message.NisCode;
                        municipality.PuriId = $"{options.Value.Namespace}/{message.Message.NisCode}";
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
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
                await context.FindAndUpdateMunicipality(
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
                await context.FindAndUpdateMunicipality(
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
                await context.FindAndUpdateMunicipality(
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
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                municipality.OfficialLanguageDutch = true;
                                break;
                            case Language.French:
                                municipality.OfficialLanguageFrench = true;
                                break;
                            case Language.German:
                                municipality.OfficialLanguageGerman = true;
                                break;
                            case Language.English:
                                municipality.OfficialLanguageEnglish = true;
                                break;
                        }

                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                municipality.OfficialLanguageDutch = false;
                                break;
                            case Language.French:
                                municipality.OfficialLanguageFrench = false;
                                break;
                            case Language.German:
                                municipality.OfficialLanguageGerman = false;
                                break;
                            case Language.English:
                                municipality.OfficialLanguageEnglish = false;
                                break;
                        }

                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                municipality.FacilityLanguageDutch = true;
                                break;
                            case Language.French:
                                municipality.FacilityLanguageFrench = true;
                                break;
                            case Language.German:
                                municipality.FacilityLanguageGerman = true;
                                break;
                            case Language.English:
                                municipality.FacilityLanguageEnglish = true;
                                break;
                        }

                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                municipality.FacilityLanguageDutch = false;
                                break;
                            case Language.French:
                                municipality.FacilityLanguageFrench = false;
                                break;
                            case Language.German:
                                municipality.FacilityLanguageGerman = false;
                                break;
                            case Language.English:
                                municipality.FacilityLanguageEnglish = false;
                                break;
                        }

                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Current;
                        municipality.OsloStatus = MunicipalityStatus.Current.ConvertFromMunicipalityStatus();
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Current;
                        municipality.OsloStatus = MunicipalityStatus.Current.ConvertFromMunicipalityStatus();
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        municipality.OsloStatus = MunicipalityStatus.Retired.ConvertFromMunicipalityStatus();
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        municipality.OsloStatus = MunicipalityStatus.Retired.ConvertFromMunicipalityStatus();
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasMerged>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateMunicipality(
                    message.Message.MunicipalityId,
                    municipality =>
                    {
                        municipality.Status = MunicipalityStatus.Retired;
                        municipality.OsloStatus = MunicipalityStatus.Retired.ConvertFromMunicipalityStatus();
                        UpdateVersionTimestamp(municipality, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });
        }

        private static void UpdateNameByLanguage(MunicipalityLatestItem municipality, Language? language, string? name)
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

        private static void UpdateVersionTimestamp(MunicipalityLatestItem municipality, Instant versionTimestamp)
            => municipality.VersionTimestamp = versionTimestamp;
    }
}
