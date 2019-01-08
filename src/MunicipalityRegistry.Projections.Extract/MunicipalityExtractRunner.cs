namespace MunicipalityRegistry.Projections.Extract
{
    using System.Text;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using Microsoft.Extensions.Logging;
    using MunicipalityExtract;

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
                new MunicipalityExtractProjection(Encoding.GetEncoding(DbaseCodePage.Western_European_ANSI.ToByte()))) { }
    }
}
