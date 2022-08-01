namespace MunicipalityRegistry.Infrastructure.Modules
{
    using Autofac;
    using Municipality;
    using Repositories;

    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
            => builder
                .RegisterType<Municipalities>()
                .As<IMunicipalities>();
    }
}
