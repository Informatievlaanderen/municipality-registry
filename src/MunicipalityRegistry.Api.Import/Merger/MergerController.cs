namespace MunicipalityRegistry.Api.Import.Merger
{
    using Asp.Versioning;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Microsoft.AspNetCore.Mvc;
    using NodaTime;
    using Projections.Legacy;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("merger")]
    [ApiExplorerSettings(GroupName = "Merger")]
    public partial class MergerController : ApiController
    {
        private readonly LegacyContext _legacyContext;
        private readonly ImportContext _importContext;
        private readonly ILifetimeScope _container;

        public MergerController(
            LegacyContext legacyContext,
            ImportContext importContext,
            ILifetimeScope container)
        {
            _legacyContext = legacyContext;
            _importContext = importContext;
            _container = container;
        }

        private Provenance CreateProvenance(string reason)
        {
            return new Provenance(
                SystemClock.Instance.GetCurrentInstant(),
                Application.MunicipalityRegistry,
                new Reason(reason),
                new Operator("OVO002949"),
                Modification.Insert,
                Organisation.DigitaalVlaanderen);
        }
    }
}
