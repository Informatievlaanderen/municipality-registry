namespace MunicipalityRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public sealed class SpatialReferenceSystemId : IntegerValueObject<SpatialReferenceSystemId>
    {
        public static SpatialReferenceSystemId Lambert72 => new SpatialReferenceSystemId(31370);

        public SpatialReferenceSystemId(int spatialReferenceSystemId) : base(spatialReferenceSystemId) { }
    }
}
