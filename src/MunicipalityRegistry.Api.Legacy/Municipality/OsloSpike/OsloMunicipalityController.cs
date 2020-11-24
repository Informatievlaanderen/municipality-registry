namespace MunicipalityRegistry.Api.Legacy.Municipality
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using Projections.Legacy;
    using Responses;
    using VDS.RDF.JsonLd;
    using VDS.RDF.JsonLd.Syntax;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("gemeenten")]
    [ApiExplorerSettings(GroupName = "Gemeenten")]
    public class OsloMunicipalityController : ApiController
    {
        /// <summary>
        /// Vraag een gemeente op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseOptions"></param>
        /// <param name="nisCode">Identificator van de gemeente.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de gemeente gevonden is.</response>
        [HttpGet("{nisCode}")]
        public async Task<IActionResult> Get(
            [FromServices] LegacyContext context,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromRoute] string nisCode,
            CancellationToken cancellationToken = default)
        {
            var municipality = await context
                .MunicipalityDetail
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.NisCode == nisCode, cancellationToken);

            if (municipality == null)
                throw new ApiException("Onbestaande gemeente.", StatusCodes.Status404NotFound);

            try
            {
                var response = new OsloMunicipalityDetailResponse(
                    responseOptions.Value.Naamruimte,
                    municipality.NisCode);

                var processorOptions = new JsonLdProcessorOptions
                {
                    ProcessingMode = JsonLdProcessingMode.JsonLd10,
                    Base = new Uri("http://127.0.0.1") // base is mandatory as option, or as @base in the context
                };

                var compacted = JsonLdProcessor.Compact(
                    JToken.FromObject(response),
                    JToken.Parse(MunicipalityDetailContextCompact),
                    processorOptions);

                return Ok(compacted);
            }
            catch (Exception e)
            {
                Debugger.Break();
                Console.WriteLine(e.ToString());
                throw;
            }

        }

        private const string MunicipalityDetailContextCompact = @"
{
    ""@context"": [
        ""https://data.vlaanderen.be/context/generiek-basis.jsonld"",
        ""https://data.vlaanderen.be/context/adresregister.jsonld"",
  	    {
            ""gemeentenaam"": {
                ""@reverse"" : ""http://data.vlaanderen.be/ns/adres#isAfgeleidVan"",
                ""@type"" : ""@id""
            }
        }
    ]
}";

    private const string MunicipalityDetailContext = @"
 {        
        ""Activiteit"": ""http://www.w3.org/ns/prov#Activity"",
        ""AdministratieveEenheid"": ""https://data.vlaanderen.be/ns/generiek#AdministratieveEenheid"",
        ""Agent"": ""http://purl.org/dc/terms/Agent"",
        ""Document"": ""http://xmlns.com/foaf/0.1/Document"",
        ""DocumentOnderdeel"": ""https://data.vlaanderen.be/ns/generiek#DocumentOnderdeel"",
        ""FormeelKader"": ""http://purl.org/vocab/cpsv#FormalFramework"",
      
        ""Hoedanigheid"": ""https://data.vlaanderen.be/ns/organisatie#Hoedanigheid"",
        ""Jurisdictie"": ""http://purl.org/dc/terms/Jurisdiction"",
        ""Object"": ""http://www.w3.org/ns/prov#Entity"",
        ""Organisatie"": ""http://www.w3.org/ns/org#Organization"",
        ""Persoon"": ""http://www.w3.org/ns/person#Person"",
        ""Plaats"": ""http://purl.org/dc/terms/Location"",
        ""Versie"": ""https://data.vlaanderen.be/ns/generiek#Versie"",
        ""VersieVolgensGeldigeTijd"": ""https://data.vlaanderen.be/ns/generiek#VersieVolgensGeldigeTijd"",
        ""VersieVolgensTransactietijd"": ""https://data.vlaanderen.be/ns/generiek#VersieVolgensTransactietijd"",
        ""Identificator.identificator"": {
            ""@id"": ""http://www.w3.org/2004/02/skos/core#notation"",
            ""@type"": ""http://www.w3.org/2000/01/rdf-schema#Literal""
        },
        ""aanschrijfvorm"": {
            ""@id"": ""http://www.w3.org/2006/vcard/ns#honorific-prefix"",
            ""@type"": ""http://www.w3.org/1999/02/22-rdf-syntax-ns#langString""
        },
        ""activiteit"": {
            ""@id"": ""http://www.w3.org/ns/prov#activity"",
            ""@type"": ""http://www.w3.org/ns/prov#Activity""
        },
        ""adres"": {
            ""@id"": ""http://www.w3.org/ns/locn#address"",
            ""@type"": ""http://www.w3.org/ns/locn#Address""
        },
        ""applicatie"": {
            ""@id"": ""http://www.w3.org/ns/prov#used"",
            ""@type"": ""http://www.w3.org/ns/prov#Entity""
        },
        ""begin"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#begin"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime""
        },
        ""beschikbaarheid"": {
            ""@id"": ""http://schema.org/hoursAvailable"",
            ""@type"": ""https://schema.org/OpeningHoursSpecification""
        },
        ""beschrijving"": {
            ""@id"": ""http://purl.org/dc/terms/description"",
            ""@type"": ""http://www.w3.org/1999/02/22-rdf-syntax-ns#langString""
        },
        ""bewerking"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#bewerking"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept""
        },
        ""contactnaam"": {
            ""@id"": ""http://xmlns.com/foaf/0.1/name"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""creatie"": {
            ""@id"": ""http://www.w3.org/ns/prov#qualifiedGeneration"",
            ""@type"": ""http://www.w3.org/ns/prov#Generation""
        },
        ""default"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#default"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#boolean""
        },
        ""einde"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#einde"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime""
        },
        ""email"": {
            ""@id"": ""http://schema.org/email"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""fax"": {
            ""@id"": ""http://schema.org/faxNumber"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""geometrie"": {
            ""@id"": ""http://www.w3.org/ns/locn#geometry"",
            ""@type"": ""http://www.w3.org/ns/locn#Geometry""
        },
        ""geplandeStart"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#geplandeStart"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime""
        },
        ""gestructureerdedentificator"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#gestructureerdedentificator"",
            ""@type"": ""https://data.vlaanderen.be/ns/generiek#GestructureerdeIdentificator""
        },
        ""gml"": {
            ""@id"": ""http://www.opengis.net/ont/geosparql#asGML"",
            ""@type"": ""http://www.w3.org/2000/01/rdf-schema#Literal""
        },
        ""handeldeInOpdrachtVan"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#handeldeInOpdrachtVan"",
            ""@type"": ""http://www.w3.org/ns/org#Organization"",
            ""@container"": ""@set""
        },
        ""identificator"": {
            ""@id"": ""http://www.w3.org/ns/adms#identifier"",
            ""@type"": ""http://www.w3.org/ns/adms#Identifier"",
            ""@container"": ""@set""
        },
        ""isGerelateerdAan"": {
            ""@id"": ""http://purl.org/dc/terms/relation"",
            ""@type"": ""http://purl.org/vocab/cpsv#FormalFramework"",
            ""@container"": ""@set""
        },
        ""isOnderdeelVan"": {
            ""@id"": ""http://purl.org/dc/terms/isPartOf"",
            ""@type"": ""http://xmlns.com/foaf/0.1/Document""
        },
        ""isTijdspecialisatieVan"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#isTijdspecialisatieVan"",
            ""@type"": ""http://www.w3.org/ns/prov#Entity""
        },
        ""lokaleIdentificator"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#lokaleIdentificator"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""methode"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#methode"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept""
        },
        ""naam"": {
            ""@id"": ""http://purl.org/dc/terms/title"",
            ""@type"": ""http://www.w3.org/1999/02/22-rdf-syntax-ns#langString""
        },
        ""naamruimte"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#naamruimte"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""nummer"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#nummer"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#integer""
        },
        ""onderwerp"": {
            ""@id"": ""http://data.europa.eu/m8g/subject"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept"",
            ""@container"": ""@set""
        },
        ""openingsuren"": {
            ""@id"": ""http://schema.org/openingHours"",
            ""@type"": ""http://www.w3.org/2000/01/rdf-schema#Literal""
        },
        ""operator"": {
            ""@id"": ""http://www.w3.org/ns/prov#wasAssociatedWith"",
            ""@type"": ""http://www.w3.org/ns/prov#Agent""
        },
        ""opheffing"": {
            ""@id"": ""http://www.w3.org/ns/prov#qualifiedInvalidation"",
            ""@type"": ""http://www.w3.org/ns/prov#Invalidation""
        },
        ""plaats"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#plaats"",
            ""@type"": ""http://purl.org/dc/terms/Location""
        },
        ""plaatsnaam"": {
            ""@id"": ""http://www.w3.org/2000/01/rdf-schema#label"",
            ""@type"": ""http://www.w3.org/1999/02/22-rdf-syntax-ns#langString"",
            ""@container"": ""@set""
        },
        ""specificatie"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#specificatie"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept""
        },
        ""status"": {
            ""@id"": ""http://www.w3.org/ns/adms#status"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept""
        },
        ""taal"": {
            ""@id"": ""http://data.europa.eu/eli/ontology#language"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept"",
            ""@container"": ""@set""
        },
        ""telefoon"": {
            ""@id"": ""http://schema.org/telephone"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""tijdstip"": {
            ""@id"": ""http://www.w3.org/ns/prov#atTime"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime""
        },
        ""toegekendDoor"": {
            ""@id"": ""http://purl.org/dc/terms/creator"",
            ""@type"": ""http://purl.org/dc/terms/Agent""
        },
        ""toegekendDoorString"": {
            ""@id"": ""http://www.w3.org/ns/adms#schemaAgency"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""toegekendOp"": {
            ""@id"": ""http://purl.org/dc/terms/issued"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime""
        },
        ""toepassingsgebied"": {
            ""@id"": ""http://data.europa.eu/m8g/territorialApplication"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept"",
            ""@container"": ""@set""
        },
        ""tussentijdstip"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#tussentijdstip"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime""
        },
        ""type"": {
            ""@id"": ""http://purl.org/dc/terms/type"",
            ""@type"": ""http://www.w3.org/2004/02/skos/core#Concept""
        },
        ""versieIdentificator"": {
            ""@id"": ""https://data.vlaanderen.be/ns/generiek#versieIdentificator"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#string""
        },
        ""website"": {
            ""@id"": ""http://xmlns.com/foaf/0.1/page"",
            ""@type"": ""http://www.w3.org/2001/XMLSchema#anyURI""
        },
        ""wkt"": {
            ""@id"": ""http://www.opengis.net/ont/geosparql#asWKT"",
            ""@type"": ""http://www.w3.org/2000/01/rdf-schema#Literal""
        }
}";
    }
}
