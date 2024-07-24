namespace MunicipalityRegistry.Projections.Integration
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Convertors;
    using Infrastructure;
    using Microsoft.Extensions.Options;
    using Municipality.Events;

    [ConnectedProjectionName("Integratie gemeente versie")]
    [ConnectedProjectionDescription("Projectie die de laatste gemeente data voor de integratie database bijhoudt.")]
    public sealed class MunicipalityVersionProjections : ConnectedProjection<IntegrationContext>
    {
        public MunicipalityVersionProjections(IOptions<IntegrationOptions> options)
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .MunicipalityVersions
                    .AddAsync(
                        new MunicipalityVersion
                        {
                            Position = message.Position,
                            MunicipalityId = message.Message.MunicipalityId,
                            NisCode = message.Message.NisCode,
                            VersionTimestamp = message.Message.Provenance.Timestamp,
                            Namespace = options.Value.Namespace,
                            PuriId = $"{options.Value.Namespace}/{message.Message.NisCode}",
                            CreatedOnTimestamp = message.Message.Provenance.Timestamp,
                            Status = MunicipalityStatus.Proposed
                        }, ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.NisCode = message.Message.NisCode;
                        x.PuriId = $"{options.Value.Namespace}/{message.Message.NisCode}";
                    },
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.NisCode = message.Message.NisCode;
                        x.PuriId = $"{options.Value.Namespace}/{message.Message.NisCode}";
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, message.Message.Name);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, message.Message.Name);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, null);
                    },
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        UpdateNameByLanguage(x, message.Message.Language, null);
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                x.OfficialLanguageDutch = true;
                                break;
                            case Language.French:
                                x.OfficialLanguageFrench = true;
                                break;
                            case Language.German:
                                x.OfficialLanguageGerman = true;
                                break;
                            case Language.English:
                                x.OfficialLanguageEnglish = true;
                                break;
                        }
                    },
                    ct);
            });

            When<Envelope<MunicipalityOfficialLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                x.OfficialLanguageDutch = false;
                                break;
                            case Language.French:
                                x.OfficialLanguageFrench = false;
                                break;
                            case Language.German:
                                x.OfficialLanguageGerman = false;
                                break;
                            case Language.English:
                                x.OfficialLanguageEnglish = false;
                                break;
                        }
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasAdded>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                x.FacilityLanguageDutch = true;
                                break;
                            case Language.French:
                                x.FacilityLanguageFrench = true;
                                break;
                            case Language.German:
                                x.FacilityLanguageGerman = true;
                                break;
                            case Language.English:
                                x.FacilityLanguageEnglish = true;
                                break;
                        }
                    },
                    ct);
            });

            When<Envelope<MunicipalityFacilityLanguageWasRemoved>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        switch (message.Message.Language)
                        {
                            case Language.Dutch:
                                x.FacilityLanguageDutch = false;
                                break;
                            case Language.French:
                                x.FacilityLanguageFrench = false;
                                break;
                            case Language.German:
                                x.FacilityLanguageGerman = false;
                                break;
                            case Language.English:
                                x.FacilityLanguageEnglish = false;
                                break;
                        }
                    },
                    ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.Status = MunicipalityStatus.Current;
                        x.OsloStatus = MunicipalityStatus.Current.ConvertFromMunicipalityStatus();
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.Status = MunicipalityStatus.Current;
                        x.OsloStatus = MunicipalityStatus.Current.ConvertFromMunicipalityStatus();
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.Status = MunicipalityStatus.Retired;
                        x.OsloStatus = MunicipalityStatus.Retired.ConvertFromMunicipalityStatus();
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.Status = MunicipalityStatus.Retired;
                        x.OsloStatus = MunicipalityStatus.Retired.ConvertFromMunicipalityStatus();
                    },
                    ct);
            });

            When<Envelope<MunicipalityWasMerged>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    x =>
                    {
                        x.Status = MunicipalityStatus.Retired;
                        x.OsloStatus = MunicipalityStatus.Retired.ConvertFromMunicipalityStatus();
                    },
                    ct);
            });
        }

        private static void UpdateNameByLanguage(MunicipalityVersion municipalityVersion, Language language, string? name)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipalityVersion.NameDutch = name;
                    break;

                case Language.French:
                    municipalityVersion.NameFrench = name;
                    break;

                case Language.German:
                    municipalityVersion.NameGerman = name;
                    break;

                case Language.English:
                    municipalityVersion.NameEnglish = name;
                    break;
            }
        }
    }
}
