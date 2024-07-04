namespace MunicipalityRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class MergeMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("033e3688-38aa-4fe2-b9df-ba656b49cf70");

        public MunicipalityId MunicipalityId { get; }

        public List<MunicipalityId> MunicipalityIdsToMergeWithWith { get; }

        public List<NisCode> NisCodesToMergeWith { get; }

        public MunicipalityId NewMunicipalityId { get; }

        public NisCode NewNisCode { get; }

        public Provenance Provenance { get; }

        public MergeMunicipality(
            MunicipalityId municipalityId,
            List<MunicipalityId> municipalityIdsToMergeWithWith,
            List<NisCode> nisCodesToMergeWith,
            MunicipalityId newMunicipalityId,
            NisCode newNisCode,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            MunicipalityIdsToMergeWithWith = municipalityIdsToMergeWithWith;
            NisCodesToMergeWith = nisCodesToMergeWith;
            NewMunicipalityId = newMunicipalityId;
            NewNisCode = newNisCode;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(MergeMunicipality)}-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return MunicipalityIdsToMergeWithWith;
            yield return NisCodesToMergeWith;
            yield return NewNisCode;
            yield return NewMunicipalityId;
        }
    }
}
