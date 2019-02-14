namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using Projections.Extract;
    using System;
    using System.IO.Compression;
    using System.Threading.Tasks;

    public class MunicipalityArchiveWriter
    {
        public Task WriteAsync(ZipArchive archive, ExtractContext context)
        {
            if (archive == null) throw new ArgumentNullException(nameof(archive));
            if (context == null) throw new ArgumentNullException(nameof(context));

            return Task.CompletedTask;
        }
    }
}
