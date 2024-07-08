namespace MunicipalityRegistry.Api.Import.Merger
{
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Be.Vlaanderen.Basisregisters.Api;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("merger")]
    [ApiExplorerSettings(GroupName = "Merger")]
    public class MergerController : ApiController
    {
        public MergerController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> Propose(
            [FromBody] ProposeMergersRequest request,
            IValidator<ProposeMergersRequest> validator,
            CancellationToken cancellationToken = default)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            return Ok();
        }
    }
}
