namespace Be.Vlaanderen.Basisregisters.Beamer
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationBuilder
    {
        public static IConfiguration Build(string appsettingsFileName)
        {
            return new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile(appsettingsFileName, optional: false, reloadOnChange: false)
                .Build();
        }
    }
}
