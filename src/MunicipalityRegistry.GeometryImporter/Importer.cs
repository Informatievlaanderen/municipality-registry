namespace MunicipalityRegistry.GeometryImporter
{
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using NetTopologySuite.IO.GML2;
    using NetTopologySuite.Operation.Valid;
    using Projections.Integration;
    using Serilog;

    public class Importer
    {
        private readonly XNamespace _vrbgNamespace = "https://geo.api.vlaanderen.be/VRBG2025";

        private const string WFS_GetMunicipalityGeometry =
            "https://geo.api.vlaanderen.be/VRBG2025/wfs?service=WFS" +
            "&version=1.1.0" +
            "&request=GetFeature" +
            "&typeName=VRBG2025:Refgem" +
            "&maxFeatures=1" +
            "&srsName=EPSG:31370" +
            "&CQL_FILTER=NISCODE=";

        private readonly IntegrationContext _integrationContext;

        public Importer(IntegrationContext integrationContext)
        {
            _integrationContext = integrationContext;
        }

        public async Task ExecuteAsync()
        {
            using var httpClient = new HttpClient();

            foreach (var nisCode in _integrationContext.MunicipalityLatestItems.Select(x => x.NisCode))
            {
                var response = await httpClient.GetAsync(WFS_GetMunicipalityGeometry + nisCode);

                var stream = await response.Content.ReadAsStreamAsync();

                var data = XDocument.Load(stream);

                var shapeElement = data.Descendants(_vrbgNamespace + "SHAPE").FirstOrDefault();

                if (shapeElement is null)
                {
                    Log.Warning($"No geometry found for niscode: {nisCode}");
                    continue;
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
                geometry.SRID = 31370;

                var validOp = new IsValidOp(geometry)
                {
                    IsSelfTouchingRingFormingHoleValid = true
                };

                if (!validOp.IsValid)
                {
                    Log.Error($"Invalid Geometry for {nisCode}");
                    break;
                }

                var municipalityGeometry = _integrationContext.MunicipalityGeometries.FirstOrDefault(x => x.NisCode == nisCode);

                if (municipalityGeometry is null)
                {
                    _integrationContext.MunicipalityGeometries.Add(new MunicipalityGeometry
                    {
                        NisCode = nisCode,
                        Geometry = geometry
                    });
                }
                else
                {
                    municipalityGeometry = _integrationContext.MunicipalityGeometries.First(x => x.NisCode == nisCode);
                    if (municipalityGeometry.Geometry == geometry)
                    {
                        continue;
                    }

                    municipalityGeometry.Geometry = geometry;
                }

                await _integrationContext.SaveChangesAsync();
                Log.Information($"Municipality {nisCode} geometry has been inserted/updated.");
            }
        }
    }
}
