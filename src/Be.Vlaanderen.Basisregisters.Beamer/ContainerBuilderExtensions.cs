namespace Be.Vlaanderen.Basisregisters.Beamer
{
    using System.IO;
    using System.Reflection;
    using Autofac;
    using Plugins;

    public static class ContainerBuilderExtensions
    {
        public static void RegisterPlugin<T>(this ContainerBuilder builder)
        {
            var type = typeof(T);
            var assembly = Assembly.GetAssembly(type);

            var assemblyConfigurationFileLocation = Path.Combine(
                Path.GetDirectoryName(assembly.Location),
                $"appsettings.{assembly.GetName().Name}.json");

            var configuration = ConfigurationBuilder.Build(Path.Combine(assemblyConfigurationFileLocation));
            if (configuration == null)
                return;

            builder.Register(c => new Plugin(type.Name, type, configuration));
        }
    }
}
