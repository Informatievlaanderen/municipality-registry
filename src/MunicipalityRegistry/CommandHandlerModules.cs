namespace MunicipalityRegistry
{
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Municipality;

    public static class CommandHandlerModules
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<MunicipalityProvenanceFactory>()
                .SingleInstance();

            containerBuilder
                .RegisterType<MunicipalityCommandHandlerModule>()
                .Named<CommandHandlerModule>(typeof(MunicipalityCommandHandlerModule).FullName)
                .As<CommandHandlerModule>();
        }
    }
}
