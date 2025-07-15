namespace MunicipalityRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class RemoveMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("52a0b963-ecad-4d86-85a3-3e2d25b526f7");

        public MunicipalityId MunicipalityId { get; }

        public Provenance Provenance { get; }

        public RemoveMunicipality(
            MunicipalityId municipalityId,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Provenance = provenance;
        }

        public Guid CreateCommandId() => Deterministic.Create(Namespace, $"{nameof(RemoveMunicipality)}-{ToString()}");

        public override string? ToString() => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
        }
    }
}
