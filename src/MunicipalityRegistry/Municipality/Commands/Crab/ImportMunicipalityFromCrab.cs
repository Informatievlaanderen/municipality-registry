namespace MunicipalityRegistry.Municipality.Commands.Crab
{
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using System;
    using System.Collections.Generic;

    public class ImportMunicipalityFromCrab : IHasCrabProvenance
    {
        private static readonly Guid Namespace = new Guid("baef796d-a297-4cb1-8019-d6ff0502e6dc");

        public NisCode NisCode { get; set; }
        public CrabLanguage? PrimaryLanguage { get; set; }
        public CrabLanguage? SecondaryLanguage { get; set; }
        public CrabLanguage? FacilityLanguage { get; set; }
        public WkbGeometry Geometry { get; set; }
        public CrabLifetime Lifetime { get; set; }
        public CrabTimestamp Timestamp { get; set; }
        public CrabOperator Operator { get; set; }
        public CrabModification? Modification { get; set; }
        public CrabOrganisation? Organisation { get; set; }
        public NumberOfFlags NumberOfFlags { get; set; }
        public CrabMunicipalityId MunicipalityId { get; set; }

        public ImportMunicipalityFromCrab(
            NisCode nisCode,
            CrabLanguage? primaryLanguage,
            CrabLanguage? secondaryLanguage,
            CrabLanguage? facilityLanguage,
            WkbGeometry geometry,
            NumberOfFlags numberOfFlags,
            CrabLifetime lifetime,
            CrabTimestamp timestamp,
            CrabOperator @operator,
            CrabModification? modification,
            CrabOrganisation? organisation,
            CrabMunicipalityId municipalityId)
        {
            NisCode = nisCode;
            PrimaryLanguage = primaryLanguage;
            SecondaryLanguage = secondaryLanguage;
            FacilityLanguage = facilityLanguage;
            Geometry = geometry;
            Lifetime = lifetime;
            Timestamp = timestamp;
            Operator = @operator;
            Modification = modification;
            Organisation = organisation;
            NumberOfFlags = numberOfFlags;
            MunicipalityId = municipalityId;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"ImportMunicipalityFromCrab-{ToString()}");

        public override string ToString() => ToStringBuilder.ToString(IdentityFields);

        private IEnumerable<object> IdentityFields()
        {
            yield return NisCode;
            yield return PrimaryLanguage;
            yield return SecondaryLanguage;
            yield return FacilityLanguage;
            yield return Geometry;
            yield return Lifetime;
            yield return Timestamp;
            yield return Operator;
            yield return Modification;
            yield return Organisation;
            yield return NumberOfFlags;
            yield return MunicipalityId;
        }
    }
}
