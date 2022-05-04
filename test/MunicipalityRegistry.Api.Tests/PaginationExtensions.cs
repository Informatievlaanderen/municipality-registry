namespace MunicipalityRegistry.Api.Tests
{
    using System;

    public static class PaginationExtensions
    {
        public static (int? offset, int? limit) ParsePaginationFromUrl(this string url)
        {
            if (!url.Contains('?'))
            {
                return (null, null);
            }

            int? offset = null;
            int? limit = null;

            var pieces = url[14..].Split('&', 2, StringSplitOptions.RemoveEmptyEntries);
            foreach (var piece in pieces)
            {
                var keyValues = piece.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                var key = keyValues[0];
                var value = keyValues[1];

                if (key == "offset")
                {
                    if (int.TryParse(value, out var newOffset))
                    {
                        offset = newOffset;
                    }
                }

                if (key == "limit")
                {
                    if (int.TryParse(value, out var newLimit))
                    {
                        limit = newLimit;
                    }
                }
            }

            return (offset, limit);
        }
    }
}
