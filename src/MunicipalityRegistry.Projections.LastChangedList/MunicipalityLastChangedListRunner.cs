namespace MunicipalityRegistry.Projections.LastChangedList
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.Extensions.Logging;

    public class MunicipalityLastChangedListRunner : LastChangedListRunner
    {
        public const string Name = "MunicipalityLastChangedListRunner";

        public MunicipalityLastChangedListRunner(
            EnvelopeFactory envelopeFactory,
            ILogger<MunicipalityLastChangedListRunner> logger) :
            base(
                Name,
                new Projections(),
                envelopeFactory,
                logger) { }
    }
}
