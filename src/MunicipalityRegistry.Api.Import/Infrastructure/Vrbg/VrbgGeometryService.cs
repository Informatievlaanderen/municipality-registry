namespace MunicipalityRegistry.Api.Import.Infrastructure.Vrbg
{
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Exceptions;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO.GML2;
    using NetTopologySuite.Operation.Valid;

    public class VrbgGeometryService: IMunicipalityGeometryReader
    {
        private readonly XNamespace _vrbgNamespace = "https://geo.api.vlaanderen.be/VRBG";

        private const string WFS_GetMunicipalityGeometry =
            "https://geo.api.vlaanderen.be/VRBG/wfs?service=WFS" +
            "&version=1.1.0" +
            "&request=GetFeature" +
            "&typeName=VRBG:Refgem" +
            "&maxFeatures=1" +
            "&srsName=EPSG:31370" +
            "&CQL_FILTER=NISCODE=";

        public async Task<Geometry> GetGeometry(string nisCode)
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(WFS_GetMunicipalityGeometry + nisCode);

            var stream = await response.Content.ReadAsStreamAsync();

            var data = XDocument.Load(stream);

            var shapeElement = data.Descendants(_vrbgNamespace + "SHAPE").FirstOrDefault();
            if (shapeElement is null)
            {
                throw new InvalidPolygonException();
            }

            var gml = shapeElement
                .Elements()
                .First();

            gml
                .DescendantsAndSelf()
                .ToList()
                .ForEach(d => d.RemoveAttributes());

            var gmlReader = new GMLReader();
            var geometry = gmlReader.Read(gml?.ToString());
            geometry.SRID = ExtendedWkbGeometry.SridLambert72;

            var validOp = new IsValidOp(geometry)
            {
                IsSelfTouchingRingFormingHoleValid = true,
                SelfTouchingRingFormingHoleValid = true
            };

            if (!validOp.IsValid)
            {
                throw new InvalidPolygonException();
            }

            return geometry;
        }
    }
}
