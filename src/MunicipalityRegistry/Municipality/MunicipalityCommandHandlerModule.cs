namespace MunicipalityRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Commands;
    using Commands.Crab;
    using SqlStreamStore;

    public sealed class MunicipalityCommandHandlerModule : CommandHandlerModule
    {
        public MunicipalityCommandHandlerModule(
            Func<IMunicipalities> getMunicipalities,
            Func<ConcurrentUnitOfWork> getUnitOfWork,
            Func<IStreamStore> getStreamStore,
            EventMapping eventMapping,
            EventSerializer eventSerializer,
            MunicipalityCrabProvenanceFactory crabProvenanceFactory,
            MunicipalityProvenanceFactory provenanceFactory)
        {
            For<ImportMunicipalityFromCrab>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, crabProvenanceFactory)
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
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, crabProvenanceFactory)
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

            For<RegisterMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipalities = getMunicipalities();

                    var municipalityId = message.Command.MunicipalityId;

                    var municipality = await municipalities.GetOptionalAsync(municipalityId, ct);

                    if (municipality.HasValue)
                        throw new AggregateSourceException($"Municipality with id {municipalityId} already exists");

                    var newMunicipality = Municipality.Register(
                        municipalityId,
                        message.Command.NisCode,
                        message.Command.OfficialLanguages,
                        message.Command.FacilityLanguages,
                        message.Command.Names,
                        message.Command.Geometry);

                    municipalities.Add(municipalityId, newMunicipality);
                });

            For<MergeMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipalities = getMunicipalities();

                    var municipalityId = message.Command.MunicipalityId;

                    var municipality = await municipalities.GetAsync(municipalityId, ct);

                    municipality.Merge(
                        message.Command.MunicipalityIdsToMergeWithWith,
                        message.Command.NisCodesToMergeWith,
                        message.Command.NewMunicipalityId,
                        message.Command.NewNisCode);
                });

            For<ActivateMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipalityId = message.Command.MunicipalityId;

                    var municipality = await getMunicipalities().GetAsync(municipalityId, ct);

                    municipality.Activate();
                });
        }
    }
}
