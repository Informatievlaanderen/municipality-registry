namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using Projections.Extract;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Extract;
    using Microsoft.EntityFrameworkCore;

    public class MunicipalityRegistryExtractBuilder
    {
        public static ExtractFile CreateMunicipalityFile(ExtractContext context)
            => CreateDbfFile<MunicipalityDbaseRecord>(
                ExtractController.ZipName,
                new MunicipalityDbaseSchema(),
                context
                    .MunicipalityExtract
                    .AsNoTracking()
                    .Select(org => org.DbaseRecord),
                context.MunicipalityExtract.Count);

        private static ExtractFile CreateDbfFile<TDbaseRecord>(
            string fileName,
            DbaseSchema schema,
            IEnumerable<byte[]> records,
            Func<int> getRecordCount) where TDbaseRecord : DbaseRecord, new()
            => new ExtractFile(
                new DbfFileName(fileName),
                (stream, token) =>
                {
                    var dbfFile = CreateDbfFileWriter<TDbaseRecord>(
                        schema,
                        new DbaseRecordCount(getRecordCount()),
                        stream);

                    foreach (var record in records)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        dbfFile.WriteBytesAs<TDbaseRecord>(record);
                    }

                    dbfFile.WriteEndOfFile();
                });

        private static DbfFileWriter<TDbaseRecord> CreateDbfFileWriter<TDbaseRecord>(
            DbaseSchema schema,
            DbaseRecordCount recordCount,
            Stream writeStream) where TDbaseRecord : DbaseRecord
            => new DbfFileWriter<TDbaseRecord>(
                new DbaseFileHeader(
                    DateTime.Now,
                    DbaseCodePage.Western_European_ANSI,
                    recordCount,
                    schema
                ),
                writeStream);
    }
}
