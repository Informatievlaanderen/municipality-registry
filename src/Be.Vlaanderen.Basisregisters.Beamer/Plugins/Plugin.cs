namespace Be.Vlaanderen.Basisregisters.Beamer.Plugins
{
    using System;
    using Microsoft.Extensions.Configuration;

    public class Plugin
    {
        public string Name { get; }
        public Type ConnectedProjectionType { get; }
        public PluginState State { get; set; }
        public string ConnectionString { get; }

        public Plugin(
            string name,
            Type connectedProjectionType,
            IConfiguration configuration)
        {
            Name = name;
            ConnectedProjectionType = connectedProjectionType;
            ConnectionString = configuration.GetConnectionString("Events");

            State = PluginState.Stopped;
        }
    }
}
