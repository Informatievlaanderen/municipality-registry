namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using System.IO;
    using System.Text;
    using Be.Vlaanderen.Basisregisters.Api.Extract;
    using Be.Vlaanderen.Basisregisters.Shaperon;

    public class DbfFileWriter<TDbaseRecord> : ExtractFileWriter
        where TDbaseRecord : DbaseRecord
    {
        private static Encoding Encoding => DbaseCodePage.Western_European_ANSI.ToEncoding();

        public DbfFileWriter(DbaseFileHeader header, Stream writeStream)
            : base(Encoding, writeStream) => header.Write(Writer);

        public void Write(TDbaseRecord record) => record.Write(Writer);

        public void WriteBytesAs<T>(byte[] recordBytes)
            where T : TDbaseRecord, new()
        {
            var record = new T();
            record.FromBytes(recordBytes, Encoding);
            Write(record);
        }

        public void WriteEndOfFile() => Writer.Write(DbaseRecord.EndOfFile);
    }

    public class DbfFileName : ExtractFileName
    {
        public DbfFileName(string name)
            : base(name, "dbf") { }
    }

    public class ShpFileName : ExtractFileName
    {
        public ShpFileName(string name)
            : base(name, "shp") { }
    }

    public class ShxFileName : ExtractFileName
    {
        public ShxFileName(string name)
            : base(name, "shx") { }
    }
}
