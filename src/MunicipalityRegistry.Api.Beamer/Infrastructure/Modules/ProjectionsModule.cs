namespace MunicipalityRegistry.Api.Beamer.Infrastructure.Modules
{
    using Be.Vlaanderen.Basisregisters.Beamer;
    using Be.Vlaanderen.Basisregisters.Beamer.Modules;
    using Autofac;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityList;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityName;
    using MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication;

    public class ProjectionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<BeamerModule>();

            builder.RegisterPlugin<MunicipalityDetailProjections>();
            builder.RegisterPlugin<MunicipalityListProjections>();
            builder.RegisterPlugin<MunicipalityNameProjections>();
            builder.RegisterPlugin<MunicipalitySyndicationProjections>();
        }
    }
}
