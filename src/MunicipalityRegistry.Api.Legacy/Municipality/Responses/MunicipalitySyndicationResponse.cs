namespace MunicipalityRegistry.Api.Legacy.Municipality.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Syndication;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.Extensions.Options;
    using Microsoft.SyndicationFeed;
    using Microsoft.SyndicationFeed.Atom;
    using Query;
    using Swashbuckle.AspNetCore.Filters;
    using Provenance = Be.Vlaanderen.Basisregisters.GrAr.Provenance.Syndication.Provenance;

    public static class MunicipalitySyndicationResponse
    {
        public static async Task WriteMunicipality(
            this ISyndicationFeedWriter writer,
            IOptions<ResponseOptions> responseOptions,
            AtomFormatter formatter,
            string category,
            MunicipalitySyndicationQueryResult municipality)
        {
            var item = new SyndicationItem
            {
                Id = municipality.Position.ToString(CultureInfo.InvariantCulture),
                Title = $"{municipality.ChangeType}-{municipality.Position}",
                Published = municipality.RecordCreatedAt.ToBelgianDateTimeOffset(),
                LastUpdated = municipality.LastChangedOn.ToBelgianDateTimeOffset(),
                Description = BuildDescription(municipality, responseOptions.Value.Naamruimte)
            };

            if (!string.IsNullOrWhiteSpace(municipality.NisCode))
            {
                item.AddLink(
                    new SyndicationLink(
                        new Uri($"{responseOptions.Value.Naamruimte}/{municipality.NisCode}"),
                        AtomLinkTypes.Related));

                //item.AddLink(
                //    new SyndicationLink(
                //        new Uri(string.Format(responseOptions.Value.DetailUrl, municipality.NisCode)),
                //        AtomLinkTypes.Self));

                //item.AddLink(
                //    new SyndicationLink(
                //        new Uri(string.Format($"{responseOptions.Value.DetailUrl}.xml", municipality.NisCode)),
                //        AtomLinkTypes.Alternate)
                //    { MediaType = MediaTypeNames.Application.Xml });

                //item.AddLink(
                //    new SyndicationLink(
                //            new Uri(string.Format($"{responseOptions.Value.DetailUrl}.json", municipality.NisCode)),
                //        AtomLinkTypes.Alternate)
                //    { MediaType = MediaTypeNames.Application.Json });
            }

            item.AddCategory(
                new SyndicationCategory(category));

            item.AddContributor(
                new SyndicationPerson(
                    municipality.Organisation?.ToName(),
                    string.Empty,
                    AtomContributorTypes.Author));

            await writer.Write(item);
        }

        private static string BuildDescription(MunicipalitySyndicationQueryResult municipality, string naamruimte)
        {
            if (!municipality.ContainsEvent && !municipality.ContainsObject)
                return "No data embedded";

            var content = new SyndicationContent();

            if (municipality.ContainsObject)
            {
                content.Object = new MunicipalitySyndicationContent(
                    municipality.MunicipalityId.Value,
                    naamruimte,
                    municipality.Status,
                    municipality.NisCode,
                    municipality.OfficialLanguages,
                    municipality.FacilitiesLanguages,
                    municipality.NameDutch,
                    municipality.NameFrench,
                    municipality.NameGerman,
                    municipality.NameEnglish,
                    municipality.LastChangedOn.ToBelgianDateTimeOffset(),
                    municipality.Organisation,
                    municipality.Reason);
            }

            if (municipality.ContainsEvent)
            {
                var doc = new XmlDocument();
                doc.LoadXml(municipality.EventDataAsXml);
                content.Event = doc.DocumentElement;
            }

            return content.ToXml();
        }
    }

    [DataContract(Name = "Content", Namespace = "")]
    public class SyndicationContent : SyndicationContentBase
    {
        [DataMember(Name = "Event", Order = 1)]
        public XmlElement Event { get; set; }

        [DataMember(Name = "Object", Order = 2)]
        public MunicipalitySyndicationContent Object { get; set; }
    }

    [DataContract(Name = "Gemeente", Namespace = "")]
    public class MunicipalitySyndicationContent
    {
        /// <summary>
        /// De technische id van de gemeente.
        /// </summary>
        [DataMember(Name = "Id", Order = 1)]
        public Guid MunicipalityId { get; private set; }

        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [DataMember(Name = "Identificator", Order = 2)]
        public GemeenteIdentificator Identificator { get; private set; }

        /// <summary>
        /// De officiële talen van de gemeente.
        /// </summary>
        [DataMember(Name = "OfficieleTalen", Order = 3)]
        public List<Taal> OfficialLanguages { get; private set; }

        /// <summary>
        /// De faciliteiten talen van de gemeente.
        /// </summary>
        [DataMember(Name = "FaciliteitenTalen", Order = 4)]
        public List<Taal> FacilitiesLanguages { get; private set; }

        /// <summary>
        /// De officiële namen van de gemeente.
        /// </summary>
        [DataMember(Name = "Gemeentenamen", Order = 5)]
        public List<GeografischeNaam> MunicipalityNames { get; private set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [DataMember(Name = "GemeenteStatus", Order = 6)]
        public GemeenteStatus? MunicipalityStatus { get; private set; }

        /// <summary>
        /// Creatie data ivm het item.
        /// </summary>
        [DataMember(Name = "Creatie", Order = 7)]
        public Provenance Provenance { get; private set; }

        public MunicipalitySyndicationContent(
            Guid municipalityId,
            string naamruimte,
            MunicipalityStatus? status,
            string nisCode,
            IEnumerable<Language> officialLanguages,
            IEnumerable<Language> facilitiesLanguages,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish,
            DateTimeOffset version,
            Organisation? organisation,
            string reason)
        {
            MunicipalityId = municipalityId;
            Identificator = new GemeenteIdentificator(naamruimte, nisCode, version);
            MunicipalityStatus = status?.ConvertFromMunicipalityStatus();
            OfficialLanguages = officialLanguages.Select(x => x.ConvertFromLanguage()).ToList();
            FacilitiesLanguages = facilitiesLanguages.Select(x => x.ConvertFromLanguage()).ToList();

            var gemeenteNamen = new List<GeografischeNaam>
            {
                new GeografischeNaam(nameDutch, Taal.NL),
                new GeografischeNaam(nameFrench, Taal.FR),
                new GeografischeNaam(nameGerman, Taal.DE),
                new GeografischeNaam(nameEnglish, Taal.EN),
            };

            MunicipalityNames = gemeenteNamen.Where(x => !string.IsNullOrEmpty(x.Spelling)).ToList();

            Provenance = new Provenance(version, organisation, new Reason(reason));
        }
    }

    public class MunicipalitySyndicationResponseExamples : IExamplesProvider<object>
    {
        private readonly ResponseOptions _responseOptions;

        public MunicipalitySyndicationResponseExamples(IOptions<ResponseOptions> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public object GetExamples()
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xmlns=""http://www.w3.org/2005/Atom"">
  <id>https://api.basisregisters.vlaanderen.be/v1/feeds/gemeenten.atom</id>
  <title>Basisregisters Vlaanderen - feed 'gemeenten'</title>
  <subtitle>Deze Atom feed geeft leestoegang tot events op de resource 'gemeenten'.</subtitle>
  <generator>Basisregisters Vlaanderen</generator>
  <rights>Gratis hergebruik volgens https://overheid.vlaanderen.be/sites/default/files/documenten/ict-egov/licenties/hergebruik/modellicentie_gratis_hergebruik_v1_0.html</rights>
  <updated>2018-10-05T14:06:53Z</updated>
  <author>
    <name>agentschap Informatie Vlaanderen</name>
    <email>informatie.vlaanderen@vlaanderen.be</email>
  </author>
  <link href=""https://api.basisregisters.dev-vlaanderen.be/v1/feeds/gemeenten"" rel=""self""/>
  <link href=""https://api.basisregisters.dev-vlaanderen.be/v1/feeds/gemeenten.atom"" rel =""alternate"" type = ""application/atom+xml"" />
  <link href=""https://api.basisregisters.dev-vlaanderen.be/v1/feeds/gemeenten.xml"" rel = ""alternate"" type = ""application/xml"" />
  <link href=""https://docs.basisregisters.dev-vlaanderen.be/"" rel = ""related"" />
  <entry>
    <id>4</id>
    <title>MunicipalityNisCodeWasDefined-4</title>
    <updated>2018-10-04T13:12:17Z</updated>
    <published>2018-10-04T13:12:17Z</published>
    <link href=""{_responseOptions.Naamruimte}/23101"" rel=""related"" />
    <author>
      <name>agentschap Informatie Vlaanderen</name>
    </author>
    <category term=""gemeenten"" />
    <content>
<![CDATA[<Content xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
<Event>
    <MunicipalityWasRegistered>
    <MunicipalityId>3793d1d5-6eb4-580c-b1db-37a8c5f43ca7</MunicipalityId><NisCode>23101</NisCode><Provenance><Timestamp>2002-08-13T15:32:32Z</Timestamp><Modification>Insert</Modification><Organisation>Ngi</Organisation><Reden>CentralManagementCrab</Reden></Provenance>
    </MunicipalityWasRegistered>
</Event>
<Object>
    <Id>3793d1d5-6eb4-580c-b1db-37a8c5f43ca7</Id>
    <Identificator><Id>http://data.vlaanderen.be/id/gemeente/23101</Id><Naamruimte>http://data.vlaanderen.be/id/gemeente</Naamruimte><ObjectId>23101</ObjectId><VersieId>2002-08-13T17:32:32+02:00</VersieId></Identificator><OfficieleTalen /><FaciliteitenTalen /><Gemeentenamen /><GemeenteStatus i:nil=""true"" /><Creatie><Organisatie>Nationaal Geografisch Instituut</Organisatie><Reden>Centrale bijhouding CRAB</Reden></Creatie>
</Object>
</Content>]]>
    </content>
  </entry>
</feed>";
        }
    }
}
