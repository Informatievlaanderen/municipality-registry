namespace MunicipalityRegistry.Municipality.Commands
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public sealed class MergeMunicipalities
    {
        public IEnumerable<MunicipalityId> MunicipalityIdsToMerge { get; }

        public MunicipalityId NewMunicipalityId { get; }

        public Provenance Provenance { get; }

        public MergeMunicipalities(
            IEnumerable<MunicipalityId> municipalityIdsToMerge,
            MunicipalityId newMunicipalityId,
            Provenance provenance)
        {
            MunicipalityIdsToMerge = municipalityIdsToMerge;
            NewMunicipalityId = newMunicipalityId;
            Provenance = provenance;
        }
    }
}
