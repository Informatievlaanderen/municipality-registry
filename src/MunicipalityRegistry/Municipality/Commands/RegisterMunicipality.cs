namespace MunicipalityRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class RegisterMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("825968af-52b4-4a23-b4bd-d6c1dc0f26d8");

        public MunicipalityId MunicipalityId { get; }

        public NisCode NisCode { get; }

        public List<Language> OfficialLanguages { get; }

        public List<Language> FacilitiesLanguages { get; }

        public List<MunicipalityName> Names { get; }

        public ExtendedWkbGeometry Geometry { get; }

        public Provenance Provenance { get; }

        public RegisterMunicipality(
            MunicipalityId municipalityId,
            NisCode nisCode,
            List<Language> officialLanguages,
            List<Language> facilitiesLanguages,
            List<MunicipalityName> names,
            ExtendedWkbGeometry geometry,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            OfficialLanguages = officialLanguages;
            FacilitiesLanguages = facilitiesLanguages;
            Names = names;
            Geometry = geometry;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(RegisterMunicipality)}-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return NisCode;
            yield return OfficialLanguages;
            yield return FacilitiesLanguages;
            yield return Names;
            yield return Geometry;
        }
    }
}
