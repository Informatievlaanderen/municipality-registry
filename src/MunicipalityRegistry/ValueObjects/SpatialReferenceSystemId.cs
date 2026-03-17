namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.NetTopology;

    public sealed class SpatialReferenceSystemId : IntegerValueObject<SpatialReferenceSystemId>
    {
        public static SpatialReferenceSystemId Lambert72 => new SpatialReferenceSystemId(SystemReferenceId.SridLambert72);

        public SpatialReferenceSystemId(int spatialReferenceSystemId) : base(spatialReferenceSystemId) { }
    }
}
