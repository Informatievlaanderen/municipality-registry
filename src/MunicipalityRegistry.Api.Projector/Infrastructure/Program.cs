namespace MunicipalityRegistry.Api.Projector.Infrastructure
{
    using System.Security.Cryptography.X509Certificates;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        private static class DevelopmentCertificate
        {
            internal const string Name = "api.dev.gemeente.basisregisters.vlaanderen.be.pfx";
            internal const string Key = "gemeenteregister!";
        }

        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => new WebHostBuilder()
                .UseDefaultForApi<Startup>(
                    httpPort: 2090,
                    httpsPort: 2444,
                    httpsCertificate: () => new X509Certificate2(DevelopmentCertificate.Name, DevelopmentCertificate.Key),
                    commandLineArgs: args);
    }
}
