namespace MunicipalityRegistry.Api.Legacy.Municipality.Responses
{
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Syndication;
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

            // TODO: Hier moet prolly version nog ergens in
            // TODO: AD: Waarschijnlijk niet hier maar wel in een sync endpoint voor externe gebruikers
            if (!string.IsNullOrWhiteSpace(municipality.NisCode))
            {
                item.AddLink(
                    new SyndicationLink(
                        new Uri($"{responseOptions.Value.Naamruimte}/{municipality.NisCode}"),
                        AtomLinkTypes.Related));

                item.AddLink(
                    new SyndicationLink(
                        new Uri(string.Format(responseOptions.Value.DetailUrl, municipality.NisCode)),
                        AtomLinkTypes.Self));

                item.AddLink(
                    new SyndicationLink(
                        new Uri(string.Format($"{responseOptions.Value.DetailUrl}.xml", municipality.NisCode)),
                        AtomLinkTypes.Alternate)
                    { MediaType = MediaTypeNames.Application.Xml });

                item.AddLink(
                    new SyndicationLink(
                            new Uri(string.Format($"{responseOptions.Value.DetailUrl}.json", municipality.NisCode)),
                        AtomLinkTypes.Alternate)
                    { MediaType = MediaTypeNames.Application.Json });
            }

            item.AddCategory(
                new SyndicationCategory(category));

            item.AddContributor(
                new SyndicationPerson(
                    "agentschap Informatie Vlaanderen",
                    "informatie.vlaanderen@vlaanderen.be",
                    AtomContributorTypes.Author));

            await writer.Write(item);
        }

        private static string BuildDescription(MunicipalitySyndicationQueryResult municipality, string naamruimte)
        {
            var content = new MunicipalitySyndicationContent(
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
                municipality.Plan);

            return content.ToXml();
        }
    }

    [DataContract(Name = "Gemeente", Namespace = "")]
    public class MunicipalitySyndicationContent : SyndicationContentBase
    {
        /// <summary>
        /// De technische id van de gemeente.
        /// </summary>
        [DataMember(Name = "Id", Order = 1)]
        public Guid MunicipalityId { get; set; }

        /// <summary>
        /// De identificator van de gemeente.
        /// </summary>
        [DataMember(Name = "Identificator", Order = 2)]
        public Identificator Identificator { get; set; }

        /// <summary>
        /// De officiële talen van de gemeente.
        /// </summary>
        [DataMember(Name = "OfficieleTalen", Order = 3)]
        public List<Taal> OfficialLanguages { get; set; }

        /// <summary>
        /// De faciliteiten talen van de gemeente.
        /// </summary>
        [DataMember(Name = "FaciliteitenTalen", Order = 4)]
        public List<Taal> FacilitiesLanguages { get; set; }

        /// <summary>
        /// De officiële namen van de gemeente.
        /// </summary>
        [DataMember(Name = "Gemeentenamen", Order = 5)]
        public List<GeografischeNaam> MunicipalityNames { get; set; }

        /// <summary>
        /// De fase in het leven van de gemeente.
        /// </summary>
        [DataMember(Name = "GemeenteStatus", Order = 6)]
        public GemeenteStatus? MunicipalityStatus { get; set; }

        /// <summary>
        /// Creatie data ivm het item.
        /// </summary>
        [DataMember(Name = "Creatie", Order = 7)]
        public Provenance Provenance { get; set; }

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
            Plan? plan)
        {
            MunicipalityId = municipalityId;
            Identificator = new Identificator(naamruimte, nisCode, version);
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

            Provenance = new Provenance(organisation, plan);
        }
    }

    public class MunicipalitySyndicationResponseExamples : IExamplesProvider
    {
        private readonly ResponseOptions _responseOptions;

        public MunicipalitySyndicationResponseExamples(IOptions<ResponseOptions> responseOptionsProvider)
            => _responseOptions = responseOptionsProvider.Value;

        public object GetExamples()
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xmlns=""http://www.w3.org/2005/Atom"">
  <id>https://basisregisters.vlaanderen/syndication/feed/municipality.atom</id>
  <title>Basisregisters Vlaanderen - Gemeenteregister</title>
  <subtitle>Basisregisters Vlaanderen stelt u in staat om alles te weten te komen rond: de Belgische gemeenten; de Belgische postcodes; de Vlaamse straatnamen; de Vlaamse adressen; de Vlaamse gebouwen en gebouweenheden; de Vlaamse percelen; de Vlaamse organisaties en organen; de Vlaamse dienstverlening.</subtitle>
  <generator uri=""https://basisregisters.vlaanderen"" version=""2.0.0.0"">Basisregisters Vlaanderen</generator>
  <rights>Copyright (c) 2017-2018, Informatie Vlaanderen</rights>
  <updated>2018-10-05T14:06:53Z</updated>
  <author>
    <name>agentschap Informatie Vlaanderen</name>
    <email>informatie.vlaanderen@vlaanderen.be</email>
  </author>
  <link href=""https://basisregisters.vlaanderen/syndication/feed/municipality.atom"" rel=""self"" />
  <link href=""https://legacy.staging-basisregisters.vlaanderen/"" rel=""related"" />
  <entry>
    <id>4</id>
    <title>MunicipalityNisCodeWasDefined-4</title>
    <updated>2018-10-04T13:12:17Z</updated>
    <published>2018-10-04T13:12:17Z</published>
    <link href=""{_responseOptions.Naamruimte}/13023"" rel=""related"" />
    <link href=""https://basisregisters.vlaanderen.be/api/v1/gemeenten/13023"" rel=""self"" />
    <link href=""https://basisregisters.vlaanderen.be/api/v1/gemeenten/13023.xml"" rel=""alternate"" type=""application/xml"" />
    <link href=""https://basisregisters.vlaanderen.be/api/v1/gemeenten/13023.json"" rel=""alternate"" type=""application/json"" />
    <author>
      <name>agentschap Informatie Vlaanderen</name>
      <email>informatie.vlaanderen@vlaanderen.be</email>
    </author>
    <category term=""https://data.vlaanderen.be/ns/gemeente"" />
    <content><![CDATA[
<GemeenteDetail>
    <GemeenteStatus>InGebruik</GemeenteStatus>
    <Identificator>
        <Id>{_responseOptions.Naamruimte}/13023</Id>
        <Naamruimte>{_responseOptions.Naamruimte}</Naamruimte>
        <ObjectId>13023</ObjectId>
        <VersieId>1</VersieId>
    </Identificator>
    <Creatie>
        <Organisatie>Nationaal Geografisch Instituut</Organisatie>
        <Plan>Centrale bijhouding CRAB</Plan>
    </Creatie>
</GemeenteDetail>]]></content>
  </entry>
</feed>";
        }
    }
}
