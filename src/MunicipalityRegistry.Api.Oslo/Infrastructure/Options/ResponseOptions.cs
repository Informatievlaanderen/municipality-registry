namespace MunicipalityRegistry.Api.Oslo.Infrastructure.Options
{
    using Projections.Feed.MunicipalityFeed;

    public class ResponseOptions
    {
        public string Naamruimte { get; set; }
        public string VolgendeUrl { get; set; }
        public string DetailUrl { get; set; }
        public string ContextUrlList { get; set; }
        public string ContextUrlDetail { get; set; }

        public string MunicipalityDetailAddressesLink { get; set; }
        public string MunicipalityDetailStreetNamesLink { get; set; }
        public string MunicipalityDetailPostInfoLink { get; set; }

        public MunicipalityFeedConfig MunicipalityFeed { get; set; }
    }
}
