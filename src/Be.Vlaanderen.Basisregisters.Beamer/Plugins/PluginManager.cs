namespace Be.Vlaanderen.Basisregisters.Beamer.Plugins
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    public class PluginManager
    {
        private readonly PluginRunner _pluginRunner;

        public IEnumerable<Plugin> Plugins { get; }

        public PluginManager(IEnumerable<Plugin> plugins, ILoggerFactory loggerFactory)
        {
            _pluginRunner = new PluginRunner(loggerFactory);
            Plugins = plugins;
        }

        public void TryStartPlugin(string name)
        {
            var plugin = Plugins.FirstOrDefault(p => p.Name == name);
            if (plugin == null || plugin.State == PluginState.Subscribed || plugin.State == PluginState.CatchingUp)
                return;

            _pluginRunner.SubscribePlugin(plugin);

            plugin.State = PluginState.Subscribed;
        }

        public void TryStopPlugin(string name)
        {
            var plugin = Plugins.FirstOrDefault(p => p.Name == name);
            if (plugin == null || plugin.State == PluginState.Stopped)
                return;

            _pluginRunner.UnsubscribePlugin(plugin);

            plugin.State = PluginState.Stopped;
        }
    }
}
