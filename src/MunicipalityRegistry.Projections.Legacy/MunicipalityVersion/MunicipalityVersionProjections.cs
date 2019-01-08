namespace MunicipalityRegistry.Projections.Legacy.MunicipalityVersion
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality.Events;
    using System;

    public class MunicipalityVersionProjections : ConnectedProjection<LegacyContext>
    {
        public MunicipalityVersionProjections()
        {
            When<Envelope<MunicipalityWasRegistered>>(async (context, message, ct) =>
            {
                var municipalityVersion = new MunicipalityVersion
                {
                    MunicipalityId = message.Message.MunicipalityId,
                    NisCode = message.Message.NisCode,
                    Position = message.Position,
                };

                municipalityVersion.ApplyProvenance(message.Message.Provenance);

                await context
                    .MunicipalityVersions
                    .AddAsync(municipalityVersion, ct);
            });

            When<Envelope<MunicipalityBecameCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => version.Status = MunicipalityStatus.Current,
                    ct);
            });

            When<Envelope<MunicipalityNameWasCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => ClearName(version, message.Message.Language),
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => SetName(version, message.Message.Language, message.Message.Name),
                    ct);
            });

            When<Envelope<MunicipalityNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => ClearName(version, message.Message.Language),
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => version.NisCode = message.Message.NisCode,
                    ct);
            });

            When<Envelope<MunicipalityNisCodeWasDefined>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => version.NisCode = message.Message.NisCode,
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => version.Status = MunicipalityStatus.Current,
                    ct);
            });

            When<Envelope<MunicipalityWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => version.Status = MunicipalityStatus.Retired,
                    ct);
            });

            When<Envelope<MunicipalityWasNamed>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => SetName(version, message.Message.Language, message.Message.Name),
                    ct);
            });

            When<Envelope<MunicipalityWasRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewMunicipalityVersion(
                    message.Message.MunicipalityId,
                    message,
                    version => version.Status = MunicipalityStatus.Retired,
                    ct);
            });
        }

        private static void SetName(MunicipalityVersion municipality, Language language, string name)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }

        private static void ClearName(MunicipalityVersion municipality, Language language)
        {
            switch (language)
            {
                case Language.Dutch:
                    municipality.NameDutch = string.Empty;
                    break;
                case Language.French:
                    municipality.NameFrench = string.Empty;
                    break;
                case Language.German:
                    municipality.NameGerman = string.Empty;
                    break;
                case Language.English:
                    municipality.NameEnglish = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }
    }
}
