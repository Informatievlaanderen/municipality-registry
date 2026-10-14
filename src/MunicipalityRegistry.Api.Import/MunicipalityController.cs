namespace MunicipalityRegistry.Api.Import
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Microsoft.AspNetCore.Mvc;
    using Municipality.Commands;
    using NodaTime;
    using Projections.Legacy;
    using Requests;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("gemeenten")]
    [ApiExplorerSettings(GroupName = "gemeenten")]
    public class MunicipalityController : ApiController
    {
        private readonly IIdempotentCommandHandler _idempotentCommandHandler;
        private readonly LegacyContext _legacyContext;

        public MunicipalityController(
            IIdempotentCommandHandler idempotentCommandHandler,
            LegacyContext legacyContext)
        {
            _idempotentCommandHandler = idempotentCommandHandler;
            _legacyContext = legacyContext;
        }

        [HttpPost("{persistentLocalId}/rename")]
        public async Task<IActionResult> Rename(
            [FromRoute] string persistentLocalId,
            [FromBody] RenameMunicipalityRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Naam))
                return BadRequest("Naam is verplicht");

            var municipality = _legacyContext.MunicipalityDetail.Single(x => x.NisCode == persistentLocalId);
            var renameMunicipality = new RenameMunicipality(
                new MunicipalityId(municipality.MunicipalityId.Value),
                new MunicipalityName(request.Naam, request.Taal.ToLanguage()),
                new Provenance(SystemClock.Instance.GetCurrentInstant(),
                    Application.MunicipalityRegistry,
                    new Reason("Hernoem gemeente"),
                    new Operator("OVO002949"),
                    Modification.Update,
                    Organisation.DigitaalVlaanderen));

            await _idempotentCommandHandler.Dispatch(
                renameMunicipality.CreateCommandId(),
                renameMunicipality,
                new Dictionary<string, object>(),
                cancellationToken);

            return Accepted();
        }

        [HttpDelete("{persistentLocalId}")]
        public async Task<IActionResult> Delete(
            [FromRoute] string persistentLocalId,
            CancellationToken cancellationToken = default)
        {
            var municipality = _legacyContext.MunicipalityDetail.Single(x => x.NisCode == persistentLocalId);
            var deleteMunicipality = new RemoveMunicipality(
                new MunicipalityId(municipality.MunicipalityId.Value),
                new Provenance(SystemClock.Instance.GetCurrentInstant(),
                    Application.MunicipalityRegistry,
                    new Reason("Verwijder gemeente"),
                    new Operator("OVO002949"),
                    Modification.Delete,
                    Organisation.DigitaalVlaanderen));

            await _idempotentCommandHandler.Dispatch(
                deleteMunicipality.CreateCommandId(),
                deleteMunicipality,
                new Dictionary<string, object>(),
                cancellationToken);

            return NoContent();
        }
    }
}
