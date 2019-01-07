namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using ExtractFiles;
    using Projections.Extract;

    public class MunicipalityRegistryExtractBuilder
    {
        public static ExtractFile CreateMunicipalityFile(IReadOnlyCollection<MunicipalityExtractItem> municipalities)
        {
            return ExtractBuilder.CreateDbfFile<MunicipalityDbaseRecord>(
                "gemeenten",
                new MunicipalityDbaseSchema(),
                municipalities
                    .Select(org => org.DbaseRecord)
                    .ToArray());
        }
    }

    public class ExtractBuilder
    {
        public static ExtractFile CreateDbfFile<TDbaseRecord>(string fileName, DbaseSchema schema, IReadOnlyCollection<byte[]> records)
            where TDbaseRecord : DbaseRecord, new()
        {
            var dbfFile = CreateEmptyDbfFile<TDbaseRecord>(
                fileName,
                schema,
                new DbaseRecordCount(records.Count));

            dbfFile.WriteBytesAs<TDbaseRecord>(records);

            return dbfFile;
        }

        private static DbfFile<TDbaseRecord> CreateEmptyDbfFile<TDbaseRecord>(string fileName, DbaseSchema schema, DbaseRecordCount recordCount)
            where TDbaseRecord : DbaseRecord
        {
            return new DbfFile<TDbaseRecord>(
                fileName,
                new DbaseFileHeader(
                    DateTime.Now,
                    DbaseCodePage.None, // TODO: this is the same code page as the old test files, to evaluate if this is important
                    recordCount,
                    schema));
        }
    }
}
