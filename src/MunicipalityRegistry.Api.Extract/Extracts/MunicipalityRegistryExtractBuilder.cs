namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Extract;
    using Be.Vlaanderen.Basisregisters.GrAr.Extracts;
    using Microsoft.EntityFrameworkCore;
    using Projections.Extract;

    public class MunicipalityRegistryExtractBuilder
    {
        public static ExtractFile CreateMunicipalityFile(ExtractContext context)
        {
            var extractItems = context
                .MunicipalityExtract
                .AsNoTracking();

            return ExtractBuilder.CreateDbfFile<MunicipalityDbaseRecord>(
                ExtractController.ZipName,
                new MunicipalityDbaseSchema(),
                extractItems.OrderBy(x => x.NisCode).Select(org => org.DbaseRecord),
                extractItems.Count);
        }
    }
}
