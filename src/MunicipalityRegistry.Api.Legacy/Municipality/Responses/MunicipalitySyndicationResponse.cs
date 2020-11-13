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
                    municipality.Organisation == null ? Organisation.Unknown.ToName() : municipality.Organisation.Value.ToName(),
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

    public class MunicipalitySyndicationResponseExamples : IExamplesProvider<XmlElement>
    {
        private const string RawXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xmlns=""http://www.w3.org/2005/Atom"">
    <id>https://api.basisregisters.vlaanderen.be/v1/feeds/gemeenten.atom</id>
    <title>Basisregisters Vlaanderen - feed 'gemeenten'</title>
    <subtitle>Deze Atom feed geeft leestoegang tot events op de resource 'gemeenten'.</subtitle>
    <generator uri=""https://basisregisters.vlaanderen.be"" version=""2.3.11.0"">Basisregisters Vlaanderen</generator>
    <rights>Gratis hergebruik volgens https://overheid.vlaanderen.be/sites/default/files/documenten/ict-egov/licenties/hergebruik/modellicentie_gratis_hergebruik_v1_0.html</rights>
    <updated>2020-10-15T08:48:33Z</updated>
    <author>
        <name>agentschap Informatie Vlaanderen</name>
        <email>informatie.vlaanderen@vlaanderen.be</email>
    </author>
    <link href=""https://api.basisregisters.vlaanderen.be/v1/feeds/gemeenten"" rel=""self"" />
    <link href=""https://api.basisregisters.vlaanderen.be/v1/feeds/gemeenten.atom"" rel=""alternate"" type=""application/atom+xml"" />
    <link href=""https://api.basisregisters.vlaanderen.be/v1/feeds/gemeenten.xml"" rel=""alternate"" type=""application/xml"" />
    <link href=""https://docs.basisregisters.vlaanderen.be/"" rel=""related"" />
    <link href=""https://api.basisregisters.vlaanderen.be/v1/feeds/gemeenten?from=2&amp;limit=100&amp;embed=event,object"" rel=""next"" />
    <entry>
        <id>0</id>
        <title>MunicipalityWasRegistered-0</title>
        <updated>2002-08-13T17:32:32+02:00</updated>
        <published>2002-08-13T17:32:32+02:00</published>
        <link href=""https://data.vlaanderen.be/id/gemeente/45057"" rel=""related"" />
        <author>
            <name>Nationaal Geografisch Instituut</name>
        </author>
        <category term=""gemeenten"" />
        <content>
            <![CDATA[<Content xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><Event><MunicipalityWasRegistered><MunicipalityId>2dcf67c9-d440-57c3-b2c5-612f03561b18</MunicipalityId><NisCode>45057</NisCode><Provenance><Timestamp>2002-08-13T15:32:32Z</Timestamp><Organisation>Ngi</Organisation><Reason>Centrale bijhouding CRAB</Reason></Provenance>
    </MunicipalityWasRegistered>
  </Event><Object><Id>2dcf67c9-d440-57c3-b2c5-612f03561b18</Id><Identificator><Id>https://data.vlaanderen.be/id/gemeente/45057</Id><Naamruimte>https://data.vlaanderen.be/id/gemeente</Naamruimte><ObjectId>45057</ObjectId><VersieId>2002-08-13T17:32:32+02:00</VersieId></Identificator><OfficieleTalen /><FaciliteitenTalen /><Gemeentenamen /><GemeenteStatus i:nil=""true"" /><Creatie><Tijdstip>2002-08-13T17:32:32+02:00</Tijdstip><Organisatie>Nationaal Geografisch Instituut</Organisatie><Reden>Centrale bijhouding CRAB</Reden></Creatie>
  </Object></Content>]]>
</content>
</entry>
<entry>
    <id>1</id>
    <title>MunicipalityOfficialLanguageWasAdded-1</title>
    <updated>2002-08-13T17:32:32+02:00</updated>
    <published>2002-08-13T17:32:32+02:00</published>
    <link href=""https://data.vlaanderen.be/id/gemeente/45057"" rel=""related"" />
    <author>
        <name>Nationaal Geografisch Instituut</name>
    </author>
    <category term=""gemeenten"" />
    <content>
        <![CDATA[<Content xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><Event><MunicipalityOfficialLanguageWasAdded><MunicipalityId>2dcf67c9-d440-57c3-b2c5-612f03561b18</MunicipalityId><Language>Dutch</Language><Provenance><Timestamp>2002-08-13T15:32:32Z</Timestamp><Organisation>Ngi</Organisation><Reason>Centrale bijhouding CRAB</Reason></Provenance>
    </MunicipalityOfficialLanguageWasAdded>
  </Event><Object><Id>2dcf67c9-d440-57c3-b2c5-612f03561b18</Id><Identificator><Id>https://data.vlaanderen.be/id/gemeente/45057</Id><Naamruimte>https://data.vlaanderen.be/id/gemeente</Naamruimte><ObjectId>45057</ObjectId><VersieId>2002-08-13T17:32:32+02:00</VersieId></Identificator><OfficieleTalen><Taal>nl</Taal></OfficieleTalen><FaciliteitenTalen /><Gemeentenamen /><GemeenteStatus i:nil=""true"" /><Creatie><Tijdstip>2002-08-13T17:32:32+02:00</Tijdstip><Organisatie>Nationaal Geografisch Instituut</Organisatie><Reden>Centrale bijhouding CRAB</Reden></Creatie>
  </Object></Content>]]>
</content>
</entry>
</feed>";

        public XmlElement GetExamples()
        {
            var example = new XmlDocument();
            example.LoadXml(RawXml);
            return example.DocumentElement;
        }
    }
}
