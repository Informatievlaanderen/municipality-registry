namespace MunicipalityRegistry.Api.Projector.Projections
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    [ApiRoute("projections")]
    public class ProjectionsController : ControllerBase
    {
        private readonly PluginManager _pluginManager;

        public ProjectionsController(PluginManager pluginManager) => _pluginManager = pluginManager;

        [HttpGet]
        public IActionResult Get()
        {
            var plugins = _pluginManager.Plugins.Select(x => new {x.Name, x.State});
            return Ok(plugins);
        }

        [HttpPost("start/{pluginName}")]
        public IActionResult Start(string pluginName)
        {
            _pluginManager.TryStartPlugin(pluginName);
            return Ok();
        }

        [HttpPost("stop/{pluginName}")]
        public IActionResult Stop(string pluginName)
        {
            _pluginManager.TryStopPlugin(pluginName);
            return Ok();
        }
    }
}
