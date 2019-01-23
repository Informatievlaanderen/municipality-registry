namespace MunicipalityRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Commands.Crab;

    public sealed class MunicipalityCommandHandlerModule : ProvenanceCommandHandlerModule<Municipality>
    {
        public MunicipalityCommandHandlerModule(
            Func<IMunicipalities> getMunicipalities,
            Func<ConcurrentUnitOfWork> getUnitOfWork,
            ReturnHandler<CommandMessage> finalHandler = null) : base(getUnitOfWork, finalHandler, new MunicipalityProvenanceFactory())
        {
            For<ImportMunicipalityFromCrab>()
                .Handle(async (message, ct) =>
                {
                    var municipalities = getMunicipalities();

                    var municipalityId = MunicipalityId.CreateFor(message.Command.MunicipalityId);

                    var municipality = await municipalities.GetOptionalAsync(municipalityId, ct);

                    if (!municipality.HasValue)
                    {
                        municipality = new Optional<Municipality>(Municipality.Register(municipalityId, message.Command.NisCode));
                        municipalities.Add(municipalityId, municipality.Value);
                    }

                    municipality.Value.ImportFromCrab(
                        message.Command.MunicipalityId,
                        message.Command.NisCode,
                        message.Command.PrimaryLanguage,
                        message.Command.SecondaryLanguage,
                        message.Command.FacilityLanguage,
                        message.Command.NumberOfFlags,
                        message.Command.Lifetime,
                        message.Command.Geometry,
                        message.Command.Timestamp,
                        message.Command.Operator,
                        message.Command.Modification,
                        message.Command.Organisation);
                });

            For<ImportMunicipalityNameFromCrab>()
                .Handle(async (message, ct) =>
                {
                    var municipalities = getMunicipalities();

                    var municipalityId = MunicipalityId.CreateFor(message.Command.MunicipalityId);

                    var municipality = await municipalities.GetAsync(municipalityId, ct);

                    municipality.ImportNameFromCrab(
                        message.Command.MunicipalityId,
                        message.Command.MunicipalityNameId,
                        message.Command.MunicipalityName,
                        message.Command.Lifetime,
                        message.Command.Timestamp,
                        message.Command.Operator,
                        message.Command.Modification,
                        message.Command.Organisation);
                });
        }
    }
}
