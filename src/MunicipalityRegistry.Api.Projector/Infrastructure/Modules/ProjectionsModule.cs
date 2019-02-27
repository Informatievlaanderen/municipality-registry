namespace MunicipalityRegistry.Api.Projector.Infrastructure.Modules
{
    using Autofac;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityList;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityName;
    using MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication;

    public class ProjectionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ProjectorModule>();

            builder.RegisterPlugin<MunicipalityDetailProjections>();
            builder.RegisterPlugin<MunicipalityListProjections>();
            builder.RegisterPlugin<MunicipalityNameProjections>();
            builder.RegisterPlugin<MunicipalitySyndicationProjections>();
        }
    }
}
