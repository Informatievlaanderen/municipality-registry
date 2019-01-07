namespace MunicipalityRegistry
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling.SqlStreamStore.Autofac;
    using Autofac;
    using Municipality;

    public static class CommandHandlerModules
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterSqlStreamStoreCommandHandler<MunicipalityCommandHandlerModule>(
                    c => handler =>
                        new MunicipalityCommandHandlerModule(
                            c.Resolve<Func<IMunicipalities>>(),
                            c.Resolve<Func<ConcurrentUnitOfWork>>(),
                            handler));
        }
    }
}
