namespace MunicipalityRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class TransformToLambert2008 : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("c1e8e093-9f71-4a5d-ab9e-4a2f718b2d4a");

        public MunicipalityId MunicipalityId { get; }
        public ExtendedWkbGeometry Geometry { get; }

        public Provenance Provenance { get; }

        public TransformToLambert2008(
            MunicipalityId municipalityId,
            ExtendedWkbGeometry geometry,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Geometry = geometry;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(TransformToLambert2008)}-{ToString()}");

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
