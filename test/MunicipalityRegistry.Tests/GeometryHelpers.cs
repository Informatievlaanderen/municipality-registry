namespace MunicipalityRegistry.Tests
{
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using NetTopologySuite.IO;
    using NetTopologySuite.IO.GML2;

    public static class GeometryHelpers
    {
        public static byte[] ExampleWkb { get; }
        public static byte[] OtherExampleWkb { get; }
        public static byte[] ExampleExtendedWkb { get; }
        public static byte[] OtherExampleExtendedWkb { get; }

        static GeometryHelpers()
        {
            var validPolygon =
                "POLYGON ((141298.83027724177 185196.03552261365, 141294.79827723652 185190.20384261012, 141296.80672523379 185188.7793306075, 141295.2384692356 185186.52896260843, 141296.27578123659 185185.72653060779, 141294.88224523515 185183.81600260362, 141296.85165324062 185182.33645060286, 141298.27155724168 185184.30649860576, 141298.47520523518 185184.18451460451, 141304.05254924297 185192.11923461035, 141298.83027724177 185196.03552261365))";
            var geometry = new WKTReader { DefaultSRID = SpatialReferenceSystemId.Lambert72 }.Read(validPolygon);
            ExampleWkb = geometry.AsBinary();
            geometry.SRID = SpatialReferenceSystemId.Lambert72;
            ExampleExtendedWkb = new WKBWriter { Strict = false, HandleSRID = true }.Write(geometry);

            var otherValidPolygon =
                "POLYGON ((141297.83027724177 185196.03552261365, 141294.79827723652 185190.20384261012, 141296.80672523379 185188.7793306075, 141295.2384692356 185186.52896260843, 141296.27578123659 185185.72653060779, 141294.88224523515 185183.81600260362, 141296.85165324062 185182.33645060286, 141297.27155724168 185184.30649860576, 141297.47520523518 185184.18451460451, 141304.05254924297 185192.11923461035, 141297.83027724177 185196.03552261365))";
            var otherGeometry = new WKTReader { DefaultSRID = SpatialReferenceSystemId.Lambert72 }.Read(otherValidPolygon);
            OtherExampleWkb = otherGeometry.AsBinary();
            otherGeometry.SRID = SpatialReferenceSystemId.Lambert72;
            OtherExampleExtendedWkb = new WKBWriter { Strict = false, HandleSRID = true }.Write(otherGeometry);
        }

        // Polygon is invalid because interior and exterior rings intersect
        public const string InValidGmlPolygon =
            "<gml:Polygon srsName=\"https://www.opengis.net/def/crs/EPSG/0/31370\" xmlns:gml=\"http://www.opengis.net/gml/3.2\">" +
            "<gml:exterior>" +
            "<gml:LinearRing>" +
            "<gml:posList>" +
            "140284.15277253836 186724.74131567031 140291.06016454101 186726.38355567306 140288.22675654292 186738.25798767805 140281.19098053873 186736.57913967967 140284.15277253836 186724.74131567031" +
            "</gml:posList>" +
            "</gml:LinearRing>" +
            "</gml:exterior>" +
            "<gml:interior>" +
            "<gml:LinearRing>" +
            "<gml:posList>" +
            "140284.15277253836 186724.74131567031 140291.06016454101 186726.38355567306 140288.22675654292 186738.25798767805 140281.19098053873 186736.57913967967 140284.15277253836 186724.74131567031" +
            "</gml:posList>" +
            "</gml:LinearRing>" +
            "</gml:interior>" +
            "</gml:Polygon>";

        public static GMLReader CreateGmlReader() =>
            new GMLReader(
                new GeometryFactory(
                    new PrecisionModel(PrecisionModels.Floating),
                    ExtendedWkbGeometry.SridLambert72,
                    new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XY)));

        public static Geometry ToGeometry(this string gml)
        {
            var gmlReader = CreateGmlReader();
            var geometry = gmlReader.Read(gml);

            geometry.SRID = SpatialReferenceSystemId.Lambert72;

            return geometry;
        }

        public static ExtendedWkbGeometry ToExtendedWkbGeometry(this string gml)
        {
            var gmlReader = CreateGmlReader();
            var geometry = gmlReader.Read(gml);

            geometry.SRID = SpatialReferenceSystemId.Lambert72;

            return new ExtendedWkbGeometry(geometry.AsBinary());
        }
    }
}
