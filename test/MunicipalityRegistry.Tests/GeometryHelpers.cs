namespace MunicipalityRegistry.Tests
{
    using NetTopologySuite.IO;

    public static class GeometryHelpers
    {
        public static byte[] ExampleWkb { get; }
        public static byte[] ExampleExtendedWkb { get; }

        static GeometryHelpers()
        {
            var validPolygon =
                "POLYGON ((141298.83027724177 185196.03552261365, 141294.79827723652 185190.20384261012, 141296.80672523379 185188.7793306075, 141295.2384692356 185186.52896260843, 141296.27578123659 185185.72653060779, 141294.88224523515 185183.81600260362, 141296.85165324062 185182.33645060286, 141298.27155724168 185184.30649860576, 141298.47520523518 185184.18451460451, 141304.05254924297 185192.11923461035, 141298.83027724177 185196.03552261365))";
            var geometry = new WKTReader { DefaultSRID = SpatialReferenceSystemId.Lambert72 }.Read(validPolygon);
            ExampleWkb = geometry.AsBinary();
            geometry.SRID = SpatialReferenceSystemId.Lambert72;
            ExampleExtendedWkb = new WKBWriter { Strict = false, HandleSRID = true }.Write(geometry);
        }
    }
}
