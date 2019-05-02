namespace MunicipalityRegistry.Infrastructure.Modules
{
    using Autofac;
    using Municipality;
    using Repositories;

    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
            => containerBuilder
                .RegisterType<Municipalities>()
                .As<IMunicipalities>();
    }
}
