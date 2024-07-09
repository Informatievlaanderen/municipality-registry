namespace MunicipalityRegistry.Api.Import.Merger
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Municipality.Commands;

    public partial class MergerController
    {
        [HttpPost("{fusieJaar}")]
        public async Task<IActionResult> Merge(
            [FromRoute(Name = "fusieJaar")] int mergerYear,
            CancellationToken cancellationToken = default)
        {
            var municipalityMergers = await _importContext.MunicipalityMergers
                .Where(x => x.Year == mergerYear)
                .ToListAsync(cancellationToken: cancellationToken);

            if (!municipalityMergers.Any())
            {
                return BadRequest($"No municipality mergers found for year {mergerYear}");
            }

            var municipalityIdToNisCodeMapping = await _legacyContext.MunicipalityDetail
                .ToDictionaryAsync(x => x.MunicipalityId!.Value, x => new NisCode(x.NisCode!), cancellationToken: cancellationToken);

            foreach (var mergersPerNewMunicipality in municipalityMergers.GroupBy(x => x.NewMunicipalityId))
            {
                var newMunicipalityId = new MunicipalityId(mergersPerNewMunicipality.Key);
                var newNisCode = municipalityIdToNisCodeMapping[newMunicipalityId];

                {
                    var activateMunicipalityCommand = new ActivateMunicipality(newMunicipalityId, CreateProvenance($"Fusie {mergerYear}"));

                    try
                    {
                        await using var scopedContainer = _container.BeginLifetimeScope();
                        var idempotentCommandHandler = scopedContainer.Resolve<IIdempotentCommandHandler>();
                        await idempotentCommandHandler.Dispatch(
                            activateMunicipalityCommand.CreateCommandId(),
                            activateMunicipalityCommand,
                            new Dictionary<string, object>(),
                            cancellationToken);
                    }
                    catch (IdempotencyException)
                    {
                        // Do nothing
                    }
                }

                foreach (var mergeMunicipality in mergersPerNewMunicipality)
                {
                    var mergeMunicipalityCommand = new MergeMunicipality(
                        new MunicipalityId(mergeMunicipality.MunicipalityId),
                        mergeMunicipality.MunicipalityIdsToMergeWith.Select(x => new MunicipalityId(x)).ToList(),
                        mergeMunicipality.MunicipalityIdsToMergeWith.Select(x => municipalityIdToNisCodeMapping[x]).ToList(),
                        newMunicipalityId,
                        newNisCode,
                        CreateProvenance($"Fusie {mergerYear}")
                    );

                    try
                    {
                        await using var scopedContainer = _container.BeginLifetimeScope();
                        var idempotentCommandHandler = scopedContainer.Resolve<IIdempotentCommandHandler>();
                        await idempotentCommandHandler.Dispatch(
                            mergeMunicipalityCommand.CreateCommandId(),
                            mergeMunicipalityCommand,
                            new Dictionary<string, object>(),
                            cancellationToken);
                    }
                    catch (IdempotencyException)
                    {
                        // Do nothing
                    }
                }
            }

            return Ok();
        }
    }
}
