namespace MunicipalityRegistry.Municipality.Commands.Crab
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class ImportMunicipalityNameFromCrab : IHasCrabProvenance
    {
        private static readonly Guid Namespace = new Guid("a8f6e092-6c8a-4505-a63f-cebd118cfcb9");

        public CrabMunicipalityId MunicipalityId { get; set; }
        public CrabMunicipalityName MunicipalityName { get; set; }
        public CrabLifetime Lifetime { get; set; }
        public CrabTimestamp Timestamp { get; set; }
        public CrabOperator Operator { get; set; }
        public CrabModification? Modification { get; set; }
        public CrabOrganisation? Organisation { get; set; }
        public CrabMunicipalityNameId MunicipalityNameId { get; set; }

        public ImportMunicipalityNameFromCrab(
            CrabMunicipalityId municipalityId,
            CrabMunicipalityName municipalityName,
            CrabLifetime lifetime,
            CrabTimestamp timestamp,
            CrabOperator @operator,
            CrabModification? modification,
            CrabOrganisation? organisation,
            CrabMunicipalityNameId municipalityNameId)
        {
            MunicipalityId = municipalityId;
            MunicipalityName = municipalityName;
            Lifetime = lifetime;
            Timestamp = timestamp;
            Operator = @operator;
            Modification = modification;
            Organisation = organisation;
            MunicipalityNameId = municipalityNameId;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"ImportMunicipalityNameFromCrab-{ToString()}");

        public override string ToString() =>
            ToStringBuilder.ToString(IdentityFields);

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return MunicipalityName;
            yield return Lifetime.BeginDateTime.Print();
            yield return Timestamp;
            yield return Operator;
            yield return Modification;
            yield return Organisation;
            yield return MunicipalityNameId;
        }
    }
}
