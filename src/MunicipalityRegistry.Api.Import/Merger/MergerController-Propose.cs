namespace MunicipalityRegistry.Api.Import.Merger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Exceptions;
    using FluentValidation;
    using Infrastructure.Vrbg;
    using Microsoft.AspNetCore.Mvc;
    using Municipality.Commands;
    using NetTopologySuite.Geometries;
    using NodaTime;
    using Propose;

    public partial class MergerController
    {
        [HttpPost("propose")]
        public async Task<IActionResult> Propose(
            [FromBody] ProposeMergersRequest request,
            [FromServices] IValidator<ProposeMergersRequest> validator,
            [FromServices] IMunicipalityGeometryReader municipalityGeometryReader,
            CancellationToken cancellationToken = default)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            foreach (var municipality in request.Municipalities)
            {
                var futureMunicipalityId = await EnsureMunicipalityExistsAndReturnMunicipalityId(
                    request.MergerYear,
                    municipality,
                    municipalityGeometryReader,
                    cancellationToken);

                var municipalitiesToMerge = municipality.MergerOf
                    .Select(niscode => _legacyContext.MunicipalityDetail.Single(x => x.NisCode == niscode))
                    .ToList();

                foreach (var municipalityToMerge in municipalitiesToMerge)
                {
                    await _importContext.MunicipalityMergers.AddAsync(new MunicipalityMerger(
                        request.MergerYear,
                        municipalityToMerge.MunicipalityId!.Value,
                        municipalitiesToMerge.Where(x => x.NisCode != municipalityToMerge.NisCode).Select(x => x.MunicipalityId!.Value),
                        futureMunicipalityId
                    ), cancellationToken);
                }

                await _importContext.SaveChangesAsync(cancellationToken);
            }

            return Ok();
        }

        private async Task<Guid> EnsureMunicipalityExistsAndReturnMunicipalityId(
            int mergerYear,
            ProposeMergerRequest municipality,
            IMunicipalityGeometryReader municipalityGeometryReader,
            CancellationToken cancellationToken)
        {
            var existingMunicipality = _legacyContext.MunicipalityDetail.SingleOrDefault(x => x.NisCode == municipality.NisCode);
            if (existingMunicipality is not null)
            {
                return existingMunicipality.MunicipalityId!.Value;
            }

            var newMunicipalityGeometry = await BuildMunicipalityGeometry(municipality, municipalityGeometryReader);

            var registerMunicipalityCommand = new RegisterMunicipality(
                new MunicipalityId(Guid.NewGuid()),
                new NisCode(municipality.NisCode),
                municipality.ProposeMunicipality!.OfficialLanguages.Select(ToLanguage).ToList(),
                municipality.ProposeMunicipality.FacilitiesLanguages.Select(ToLanguage).ToList(),
                municipality.ProposeMunicipality.Names.Select(n => new MunicipalityName(n.Value, ToLanguage(n.Key))).ToList(),
                ExtendedWkbGeometry.CreateEWkb(newMunicipalityGeometry.ToBinary())!,
                CreateProvenance($"Fusie {mergerYear}")
            );

            await using var scopedContainer = _container.BeginLifetimeScope();
            var idempotentCommandHandler = scopedContainer.Resolve<IIdempotentCommandHandler>();
            await idempotentCommandHandler.Dispatch(
                registerMunicipalityCommand.CreateCommandId(),
                registerMunicipalityCommand,
                new Dictionary<string, object>(),
                cancellationToken);

            return registerMunicipalityCommand.MunicipalityId;
        }

        private static async Task<Geometry> BuildMunicipalityGeometry(ProposeMergerRequest municipality, IMunicipalityGeometryReader municipalityGeometryReader)
        {
            var geometryFactory = GeometryConfiguration.CreateGeometryFactory();

            try
            {
                var newMunicipalityGeometry = await municipalityGeometryReader.GetGeometry(municipality.NisCode);
                return newMunicipalityGeometry;
            }
            catch (InvalidPolygonException)
            { }

            var municipalityGeometriesToMerge =
                await Task.WhenAll(municipality.MergerOf.Select(municipalityGeometryReader.GetGeometry));
            var newMunicipalityCombinedGeometry = new MultiPolygon(
                municipalityGeometriesToMerge.SelectMany(geometry =>
                    {
                        return geometry switch
                        {
                            MultiPolygon multiPolygon => multiPolygon.Geometries.Cast<Polygon>(),
                            Polygon polygon => new[] { polygon },
                            _ => throw new InvalidPolygonException()
                        };
                    })
                    .ToArray(),
                geometryFactory)
            {
                SRID = geometryFactory.SRID
            };

            return newMunicipalityCombinedGeometry;
        }

        private static Language ToLanguage(Taal taal)
        {
            return taal switch
            {
                Taal.NL => Language.Dutch,
                Taal.FR => Language.French,
                Taal.DE => Language.German,
                Taal.EN => Language.English,
                _ => throw new ArgumentOutOfRangeException(nameof(taal), taal, $"Non existing language '{taal}'.")
            };
        }
    }
}
