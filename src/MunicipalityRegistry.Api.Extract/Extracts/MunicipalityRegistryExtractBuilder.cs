namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Extract;
    using Be.Vlaanderen.Basisregisters.GrAr.Extracts;
    using Microsoft.EntityFrameworkCore;
    using Projections.Extract;
    using Projections.Extract.MunicipalityExtract;

    public static class MunicipalityRegistryExtractBuilder
    {
        public static IEnumerable<ExtractFile> CreateMunicipalityFiles(ExtractContext context)
        {
            var extractItems = context
                .MunicipalityExtract
                .OrderBy(x => x.NisCode)
                .AsNoTracking();

            var municipalityProjectionState = context
                .ProjectionStates
                .AsNoTracking()
                .Single(m => m.Name == typeof(MunicipalityExtractProjections).FullName);
            var extractMetadata = new Dictionary<string,string>
            {
                { ExtractMetadataKeys.LatestEventId, municipalityProjectionState.Position.ToString()},
            };

            yield return ExtractBuilder.CreateDbfFile<MunicipalityDbaseRecord>(
                ExtractController.ZipName,
                new MunicipalityDbaseSchema(),
                extractItems.OrderBy(x => x.NisCode).Select(org => org.DbaseRecord),
                extractItems.Count);

            yield return ExtractBuilder.CreateMetadataDbfFile(
                ExtractController.ZipName,
                extractMetadata);
        }
    }
}
