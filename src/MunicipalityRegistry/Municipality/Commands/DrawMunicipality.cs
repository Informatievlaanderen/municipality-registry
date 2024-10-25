namespace MunicipalityRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class DrawMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("ad280b4f-a85d-48ff-a640-ad2de0676801");

        public MunicipalityId MunicipalityId { get; }
        public ExtendedWkbGeometry Geometry { get; }

        public Provenance Provenance { get; }

        public DrawMunicipality(
            MunicipalityId municipalityId,
            ExtendedWkbGeometry geometry,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Geometry = geometry;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(DrawMunicipality)}-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Geometry.ToString();
            yield return Provenance.Timestamp;
        }
    }
}
