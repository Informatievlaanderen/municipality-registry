namespace MunicipalityRegistry.Projections.Legacy
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.Extensions.Logging;
    using MunicipalityDetail;
    using MunicipalityList;
    using MunicipalityName;
    using MunicipalitySyndication;
    using MunicipalityVersion;

    public class MunicipalityLegacyRunner : Runner<LegacyContext>
    {
        public const string Name = "MunicipalityLegacyRunner";

        public MunicipalityLegacyRunner(
            EnvelopeFactory envelopeFactory,
            ILogger<MunicipalityLegacyRunner> logger) :
            base(
                Name,
                envelopeFactory,
                logger,
                new MunicipalityListProjections(),
                new MunicipalityDetailProjections(),
                new MunicipalitySyndicationProjections(),
                new MunicipalityNameProjections(),
                new MunicipalityVersionProjections())
        { }
    }
}
