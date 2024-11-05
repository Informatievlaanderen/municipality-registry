namespace MunicipalityRegistry.Api.Import.Geometries
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Infrastructure.Vrbg;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Municipality.Commands;
    using NodaTime;
    using Projections.Legacy;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("geometries")]
    [ApiExplorerSettings(GroupName = "Geometries")]
    public sealed class GeometriesController : ApiController
    {
        private readonly ILifetimeScope _container;
        private readonly LegacyContext _legacyContext;
        private readonly IMunicipalityGeometryReader _municipalityGeometryReader;

        public GeometriesController(
            ILifetimeScope container,
            LegacyContext legacyContext,
            IMunicipalityGeometryReader municipalityGeometryReader)
        {
            _container = container;
            _legacyContext = legacyContext;
            _municipalityGeometryReader = municipalityGeometryReader;
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(CancellationToken cancellationToken = default)
        {
            var municipalitiesToUpdate = _legacyContext
                .MunicipalityList
                .Where(x => x.Status != MunicipalityStatus.Retired)
                .AsNoTracking()
                .Select(x =>
                new
                {
                    x.MunicipalityId,
                    x.NisCode
                })
                .ToList()
                .Where(x => RegionFilter.IsFlemishRegion(x.NisCode!))
                .ToList();

            foreach (var municipality in municipalitiesToUpdate)
            {
                var geometry = await _municipalityGeometryReader.GetGeometry(municipality.NisCode!);
                var drawCommand = new DrawMunicipality(
                    new MunicipalityId(municipality.MunicipalityId!.Value),
                    ExtendedWkbGeometry.CreateEWkb(geometry.ToBinary())!,
                    CreateProvenance("update geometry"));

                await using var scope = _container.BeginLifetimeScope();
                await scope.Resolve<IIdempotentCommandHandler>()
                    .Dispatch(drawCommand.CreateCommandId(),
                        drawCommand,
                        new Dictionary<string, object>(),
                        cancellationToken);
            }

            return Accepted();
        }

        private Provenance CreateProvenance(string reason)
        {
            return new Provenance(
                SystemClock.Instance.GetCurrentInstant(),
                Application.MunicipalityRegistry,
                new Reason(reason),
                new Operator("OVO002949"),
                Modification.Update,
                Organisation.DigitaalVlaanderen);
        }
    }
}
