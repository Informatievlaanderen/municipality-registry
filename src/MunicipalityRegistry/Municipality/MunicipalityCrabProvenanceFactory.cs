namespace MunicipalityRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public sealed class MunicipalityCrabProvenanceFactory : CrabProvenanceFactory, IProvenanceFactory<Municipality>
    {
        public bool CanCreateFrom<TCommand>() => typeof(IHasCrabProvenance).IsAssignableFrom(typeof(TCommand));

        public Provenance CreateFrom(object provenanceHolder, Municipality aggregate)
        {
            if (!(provenanceHolder is IHasCrabProvenance crabProvenance))
            {
                throw new InvalidOperationException($"Cannot create provenance from {provenanceHolder.GetType().Name}");
            }

            return CreateFrom(
                aggregate.LastModificationBasedOnCrab,
                crabProvenance.Timestamp,
                crabProvenance.Modification,
                crabProvenance.Operator,
                crabProvenance.Organisation);
        }
    }
}
