namespace MunicipalityRegistry.Api.IntegrationTests
{
    using System;

    public static class UriExtensions
    {
        public static bool IsValid(this Uri? uri) => uri == null || !Uri.IsWellFormedUriString(uri.ToString(), UriKind.Absolute);
    }
}
