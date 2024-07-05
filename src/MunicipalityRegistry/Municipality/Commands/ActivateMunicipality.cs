namespace MunicipalityRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class ActivateMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("8ee65e8e-8b7d-42df-abff-6c6f47ed74ed");

        public MunicipalityId MunicipalityId { get; }

        public Provenance Provenance { get; }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(ActivateMunicipality)}-{ToString()}");

        public ActivateMunicipality(
            MunicipalityId municipalityId,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Provenance = provenance;
        }

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
        }
    }
}
