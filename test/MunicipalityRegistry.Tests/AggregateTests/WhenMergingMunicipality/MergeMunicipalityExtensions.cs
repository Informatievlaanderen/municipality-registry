namespace MunicipalityRegistry.Tests.AggregateTests.WhenMergingMunicipality;

using System.Collections.Generic;
using Municipality.Commands;

public static class MergeMunicipalityExtensions
{
    public static MergeMunicipality WithMunicipalityIdsToMerge(this MergeMunicipality command, List<MunicipalityId> municipalityIdsToMergeWith)
    {
        return new MergeMunicipality(
            command.MunicipalityId,
            municipalityIdsToMergeWith,
            command.NisCodesToMergeWith,
            command.NewMunicipalityId,
            command.NewNisCode,
            command.Provenance);
    }

    public static MergeMunicipality WithNisCodesToMerge(this MergeMunicipality command, List<NisCode> nisCodesToMergeWith)
    {
        return new MergeMunicipality(
            command.MunicipalityId,
            command.MunicipalityIdsToMergeWithWith,
            nisCodesToMergeWith,
            command.NewMunicipalityId,
            command.NewNisCode,
            command.Provenance);
    }

    public static MergeMunicipality WithNewMunicipalityId(this MergeMunicipality command, MunicipalityId newMunicipalityId)
    {
        return new MergeMunicipality(
            command.MunicipalityId,
            command.MunicipalityIdsToMergeWithWith,
            command.NisCodesToMergeWith,
            newMunicipalityId,
            command.NewNisCode,
            command.Provenance);
    }

    public static MergeMunicipality WithNewNisCode(this MergeMunicipality command, NisCode newNisCode)
    {
        return new MergeMunicipality(
            command.MunicipalityId,
            command.MunicipalityIdsToMergeWithWith,
            command.NisCodesToMergeWith,
            command.NewMunicipalityId,
            newNisCode,
            command.Provenance);
    }
}
