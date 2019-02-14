namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using ExtractFiles;
    using Projections.Extract;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class MunicipalityRegistryExtractBuilder
    {
        public static ExtractFile CreateMunicipalityFile(ExtractContext context)
        {
            return CreateDbfFile<MunicipalityDbaseRecord>(
                ExtractController.ZipName,
                new MunicipalityDbaseSchema(),
                context
                    .MunicipalityExtract
                    .Select(org => org.DbaseRecord),
                context.MunicipalityExtract.Count);
        }

        private static ExtractFile CreateDbfFile<TDbaseRecord>(
           string fileName,
           DbaseSchema schema,
           IEnumerable<byte[]> records,
           Func<int> getRecordCount
       ) where TDbaseRecord : DbaseRecord, new()
        {
            return new ExtractFile(
                new DbfFileName(fileName),
                (stream, token) =>
                {
                    var dbfFile = CreateDbfFileWriter<TDbaseRecord>(
                        schema,
                        new DbaseRecordCount(getRecordCount()),
                        stream
                    );

                    foreach (var record in records)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        dbfFile.WriteBytesAs<TDbaseRecord>(record);
                    }
                    dbfFile.WriteEndOfFile();
                }
            );
        }

        private static DbfFileWriter<TDbaseRecord> CreateDbfFileWriter<TDbaseRecord>(
            DbaseSchema schema,
            DbaseRecordCount recordCount,
            Stream writeStream
        ) where TDbaseRecord : DbaseRecord
        {
            return new DbfFileWriter<TDbaseRecord>(
                new DbaseFileHeader(
                    DateTime.Now,
                    DbaseCodePage.Western_European_ANSI,
                    recordCount,
                    schema
                ),
                writeStream
            );
        }
    }
}
