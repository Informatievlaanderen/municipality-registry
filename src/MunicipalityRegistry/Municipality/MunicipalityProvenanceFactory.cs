namespace MunicipalityRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using NodaTime;

    public sealed class MunicipalityProvenanceFactory : IProvenanceFactory<Municipality>
    {
        public bool CanCreateFrom<TCommand>() => typeof(IHasProvenance).IsAssignableFrom(typeof(TCommand));

        public Provenance CreateFrom(object provenanceHolder, Municipality aggregate)
        {
            if (provenanceHolder is not IHasCommandProvenance provenance)
            {
                throw new InvalidOperationException($"Cannot create provenance from {provenanceHolder.GetType().Name}");
            }

            return new Provenance(
                SystemClock.Instance.GetCurrentInstant(),
                provenance.Provenance.Application,
                provenance.Provenance.Reason,
                provenance.Provenance.Operator,
                provenance.Provenance.Modification,
                provenance.Provenance.Organisation);
        }
    }
}
