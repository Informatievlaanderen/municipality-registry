namespace MunicipalityRegistry
{
    using NetTopologySuite;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using NetTopologySuite.IO;

    // ReSharper disable once InconsistentNaming
    public static class WKBReaderFactory
    {
        public static WKBReader Create() =>
            new WKBReader(
                new NtsGeometryServices(
                    new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XY),
                    new PrecisionModel(PrecisionModels.Floating),
                    ExtendedWkbGeometry.SridLambert72));
    }
}
