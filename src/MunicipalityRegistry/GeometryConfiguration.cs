namespace MunicipalityRegistry
{
    using NetTopologySuite;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using NetTopologySuite.IO;

    // ReSharper disable once InconsistentNaming
    public static class GeometryConfiguration
    {
        public static WKBReader CreateWkbReader() =>
            new WKBReader(CreateGeometryFactory().GeometryServices);

        public static GeometryFactory CreateGeometryFactory() =>
            new GeometryFactory(
                new PrecisionModel(PrecisionModels.Floating),
                ExtendedWkbGeometry.SridLambert72,
                new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XY));
    }
}
