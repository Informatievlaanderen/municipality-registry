namespace MunicipalityRegistry.Api.Extract.Extracts
{
    using ExtractFiles;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class MunicipalityRegistryExtractArchive
    {
        public static FileResult CreateResponse(this List<ExtractFile> files, string name, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            name = name.EndsWith(".zip")
                ? name
                : $"{name.TrimEnd('.')}.zip";

            return CreateCallbackFileStreamResult(name, files, token);
        }

        private static FileResult CreateCallbackFileStreamResult(string fileName, List<ExtractFile> files, CancellationToken token)
        {
            return new FileCallbackResult(
                new MediaTypeHeaderValue("application/octet-stream"),
                (stream, _) => Task.Run(() => WriteArchiveContent(stream, files, token), token)
            )
            {
                FileDownloadName = fileName
            };
        }

        private static void WriteArchiveContent(Stream archiveStream, List<ExtractFile> files, CancellationToken token)
        {
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
            {
                foreach (var file in files.Where(file => null != file))
                {
                    if (token.IsCancellationRequested)
                        break;

                    using (var dbfFileStream = archive.CreateEntry(file.Name).Open())
                    {
                        file.WriteTo(dbfFileStream, token);
                    }
                }
            }
        }
    }
}
