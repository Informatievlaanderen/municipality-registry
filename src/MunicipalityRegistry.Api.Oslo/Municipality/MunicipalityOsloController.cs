namespace MunicipalityRegistry.Api.Oslo.Municipality
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Asp.Versioning;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.Api.Syndication;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Syndication;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using CloudNative.CloudEvents;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.OutputCaching;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.SyndicationFeed;
    using Microsoft.SyndicationFeed.Atom;
    using Projections.Feed;
    using Projections.Legacy;
    using Query;
    using Responses;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("gemeenten")]
    [ApiExplorerSettings(GroupName = "Gemeenten")]
    public class MunicipalityOsloController : ApiController
    {
        /// <summary>
        /// Vraag een gemeente op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseOptions"></param>
        /// <param name="nisCode">Identificator van de gemeente.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de gemeente gevonden is.</response>
        /// <response code="404">Als de gemeente niet gevonden kan worden.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("{nisCode}")]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(MunicipalityOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(MunicipalityNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Get(
            [FromServices] LegacyContext context,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromRoute] string nisCode,
            CancellationToken cancellationToken = default)
        {
            var municipality =
                await context
                    .MunicipalityDetail
                    .AsNoTracking()
                    .SingleOrDefaultAsync(item => item.NisCode == nisCode, cancellationToken);

            if (municipality == null)
                throw new ApiException("Onbestaande gemeente.", StatusCodes.Status404NotFound);

            if(municipality.IsRemoved)
                throw new ApiException("Verwijderde gemeente.", StatusCodes.Status410Gone);

            return Ok(
                new MunicipalityOsloResponse(
                    responseOptions.Value.Naamruimte,
                    responseOptions.Value.ContextUrlDetail,
                    municipality.Status.ConvertFromMunicipalityStatus(),
                    municipality.NisCode,
                    municipality.OfficialLanguages,
                    municipality.FacilitiesLanguages,
                    municipality.NameDutch,
                    municipality.NameFrench,
                    municipality.NameGerman,
                    municipality.NameEnglish,
                    municipality.VersionTimestamp.ToBelgianDateTimeOffset(),
                    responseOptions.Value.DetailUrl,
                    responseOptions.Value.MunicipalityDetailStreetNamesLink,
                    responseOptions.Value.MunicipalityDetailAddressesLink,
                    responseOptions.Value.MunicipalityDetailPostInfoLink));
        }

        /// <summary>
        /// Vraag een lijst met actieve gemeenten op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="taal">Gewenste taal van de respons.</param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met gemeenten gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(MunicipalityListOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityListOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> List(
            [FromServices] LegacyContext context,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            Taal? taal,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<MunicipalityListFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedMunicipalities = new MunicipalityListOsloQuery(context).Fetch(filtering, sorting, pagination);
            var municipalities = await pagedMunicipalities.Items.ToListAsync(cancellationToken);

            Response.AddPagedQueryResultHeaders(pagedMunicipalities);

            return Ok(
                new MunicipalityListOsloResponse
                {
                    Context = responseOptions.Value.ContextUrlList,
                    Gemeenten = municipalities
                        .Select(m => new MunicipalityListOsloItemResponse(
                            m.NisCode,
                            responseOptions.Value.Naamruimte,
                            responseOptions.Value.DetailUrl,
                            m.VersionTimestamp.ToBelgianDateTimeOffset(),
                            new GeografischeNaam(m.DefaultName, m.OfficialLanguages.FirstOrDefault().ConvertFromLanguage()),
                            m.Status))
                        .ToList(),
                    Volgende = BuildNextUri(pagedMunicipalities.PaginationInfo, municipalities.Count, responseOptions.Value.VolgendeUrl)
                });
        }

        /// <summary>
        /// Vraag het totaal aantal van actieve gemeenten op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("totaal-aantal")]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Count(
            [FromServices] LegacyContext context,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<MunicipalityListFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = new NoPaginationRequest();

            return Ok(
                new TotaalAantalResponse
                {
                    Aantal = filtering.ShouldFilter
                        ? await new MunicipalityListOsloQuery(context)
                            .Fetch(filtering, sorting, pagination)
                            .Items
                            .CountAsync(cancellationToken)
                        : await context
                            .MunicipalityList
                            .CountAsync(cancellationToken)
                });
        }

        /// <summary>
        /// Vraag wijzigingen van alle gemeenten op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="page">Page in route is gebruikt voor cache opvulling.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("wijzigingen")]
        [Produces("application/cloudevents-batch+json")]
        [OutputCache(
            VaryByQueryKeys = ["page"],
            VaryByHeaderNames = [ExtractFilteringRequestExtension.HeaderName])]
        [ProducesResponseType(typeof(List<CloudEvent>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityFeedResultExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Changes(
            [FromServices] FeedContext context,
            [FromRoute] int? page,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<MunicipalityFeedFilter>();
            if(page is null)
                page = filtering.Filter?.Page ?? 1;

            var feedPosition = filtering.Filter?.FeedPosition;

            if (feedPosition.HasValue && filtering.Filter?.Page.HasValue == false)
            {
                page = context.MunicipalityFeed
                    .Where(x => x.Position == feedPosition.Value)
                    .Select(x => x.Page)
                    .Distinct()
                    .AsEnumerable()
                    .DefaultIfEmpty(1)
                    .Min();
            }

            var feedItemsEvents = await context
                .MunicipalityFeed
                .Where(x => x.Page == page)
                .OrderBy(x => x.Id)
                .Select(x => x.CloudEventAsString)
                .ToListAsync(cancellationToken);

            var jsonContent = "[" + string.Join(",", feedItemsEvents) + "]";

            Response.Headers.Append("X-Page-Complete", (feedItemsEvents.Count >= 100).ToString());

            return Content(jsonContent, "application/cloudevents-batch+json");
        }

        /// <summary>
        /// Vraag wijzigingen van een bepaalde gemeente op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nisCode">NisCode van de gemeente</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{nisCode}/wijzigingen")]
        [Produces("application/cloudevents-batch+json")]
        [ProducesResponseType(typeof(List<CloudEvent>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalityFeedResultExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> ChangesByNisCode(
            [FromServices] FeedContext context,
            [FromRoute] string nisCode,
            CancellationToken cancellationToken = default)
        {
            var pagination = (PaginationRequest)Request.ExtractPaginationRequest();

            var feedItemsEvents = await context
                .MunicipalityFeed
                .Where(x => x.NisCode == nisCode)
                .OrderBy(x => x.Id)
                .Select(x => x.CloudEventAsString)
                .Skip(pagination.Offset)
                .Take(pagination.Limit)
                .ToListAsync(cancellationToken);

            var jsonContent = "[" + string.Join(",", feedItemsEvents) + "]";

            return new ChangeFeedResult(jsonContent,  feedItemsEvents.Count >= 100);
        }

        /// <summary>
        /// Vraag een lijst met wijzigingen van gemeenten op.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("sync")]
        [Produces("text/xml")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MunicipalitySyndicationResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Sync(
            [FromServices] IConfiguration configuration,
            [FromServices] LegacyContext context,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<MunicipalitySyndicationFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var lastFeedUpdate = await context
                .MunicipalitySyndication
                .AsNoTracking()
                .OrderByDescending(item => item.Position)
                .Select(item => item.SyndicationItemCreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastFeedUpdate == default)
            {
                lastFeedUpdate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            }

            var pagedMunicipalities =
                new MunicipalitySyndicationQuery(
                    context,
                    filtering.Filter?.Embed ?? new SyncEmbedValue())
                .Fetch(filtering, sorting, pagination);

            return new ContentResult
            {
                Content = await BuildAtomFeed(lastFeedUpdate, pagedMunicipalities, responseOptions, configuration),
                ContentType = MediaTypeNames.Text.Xml,
                StatusCode = StatusCodes.Status200OK
            };
        }

        private static async Task<string> BuildAtomFeed(
            DateTimeOffset lastUpdate,
            PagedQueryable<MunicipalitySyndicationQueryResult> pagedMunicipalities,
            IOptions<ResponseOptions> responseOptions,
            IConfiguration configuration)
        {
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings { Async = true, Indent = true, Encoding = sw.Encoding }))
            {
                var formatter = new AtomFormatter(null, xmlWriter.Settings) { UseCDATA = true };
                var writer = new AtomFeedWriter(xmlWriter, null, formatter);
                var syndicationConfiguration = configuration.GetSection("Syndication");
                var atomConfiguration = AtomFeedConfigurationBuilder.CreateFrom(syndicationConfiguration, lastUpdate);

                await writer.WriteDefaultMetadata(atomConfiguration);

                var municipalities = await pagedMunicipalities.Items.ToListAsync();

                var highestPosition = municipalities.Any()
                    ? municipalities.Max(x => x.Position)
                    : (long?)null;

                var nextUri = BuildNextSyncUri(
                    pagedMunicipalities.PaginationInfo.Limit,
                    highestPosition + 1,
                    syndicationConfiguration["NextUri"]);

                if (nextUri != null)
                {
                    await writer.Write(new SyndicationLink(nextUri, GrArAtomLinkTypes.Next));
                }

                foreach (var municipality in municipalities)
                {
                    await writer.WriteMunicipality(responseOptions, formatter, syndicationConfiguration["Category"], municipality);
                }

                xmlWriter.Flush();
            }

            return sw.ToString();
        }

        private static Uri? BuildNextUri(PaginationInfo paginationInfo, int itemsInCollection, string nextUrlBase)
        {
            var offset = paginationInfo.Offset;
            var limit = paginationInfo.Limit;

            return paginationInfo.HasNextPage(itemsInCollection)
                ? new Uri(string.Format(nextUrlBase, offset + limit, limit))
                : null;
        }

        private static Uri? BuildNextSyncUri(int limit, long? from, string nextUrlBase)
        {
            return from.HasValue
                ? new Uri(string.Format(nextUrlBase, from.Value, limit))
                : null;
        }
    }
}
