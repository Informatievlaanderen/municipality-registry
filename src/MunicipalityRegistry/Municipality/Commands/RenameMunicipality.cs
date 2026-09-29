namespace MunicipalityRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class RenameMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("3f814f17-f8bc-4c8c-a7aa-f3122567a29a");

        public MunicipalityId MunicipalityId { get; }
        public MunicipalityName NewName { get; }
        public Provenance Provenance { get; }

        public RenameMunicipality(MunicipalityId municipalityId, MunicipalityName newName, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            NewName = newName;
            Provenance = provenance;
        }

        public Guid CreateCommandId() => Deterministic.Create(Namespace, $"{nameof(RenameMunicipality)}-{ToString()}");

        public override string? ToString() => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return NewName;
            yield return Provenance.Timestamp;
        }
    }
}
