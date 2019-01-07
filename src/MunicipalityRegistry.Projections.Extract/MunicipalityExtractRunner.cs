namespace MunicipalityRegistry.Projections.Extract
{
    using System.Text;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.Extensions.Logging;

    public class MunicipalityExtractRunner : Runner<ExtractContext>
    {
        public const string Name = "MunicipalityExtractRunner";

        public MunicipalityExtractRunner(
            EnvelopeFactory envelopeFactory,
            ILogger<MunicipalityExtractRunner> logger) :
            base(
                Name,
                envelopeFactory,
                logger,
                new MunicipalityExtractProjection(Encoding.GetEncoding(1252))) { }
    }
}
