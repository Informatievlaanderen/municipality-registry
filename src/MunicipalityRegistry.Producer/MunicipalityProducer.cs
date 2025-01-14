namespace MunicipalityRegistry.Producer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Projector.ConnectedProjections;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public sealed class MunicipalityProducer : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<MunicipalityProducer> _logger;
        private readonly IConnectedProjectionsManager _projectionManager;

        public MunicipalityProducer(
            IHostApplicationLifetime hostApplicationLifetime,
            IConnectedProjectionsManager projectionManager,
            ILogger<MunicipalityProducer> logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _projectionManager = projectionManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _projectionManager.Start(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, $"Critical error occured in {nameof(MunicipalityProducer)}.");
                _hostApplicationLifetime.StopApplication();
                throw;
            }
        }
    }
}
