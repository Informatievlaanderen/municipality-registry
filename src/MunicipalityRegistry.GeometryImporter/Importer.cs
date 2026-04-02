namespace MunicipalityRegistry.GeometryImporter
{
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.NetTopology;
    using Microsoft.EntityFrameworkCore;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO.GML2;
    using NetTopologySuite.Operation.Valid;
    using Projections.Integration;
    using Serilog;

    public class Importer
    {
        private const int SrIdLambert72 = SystemReferenceId.SridLambert72;
        private const int SrIdLambert08 = SystemReferenceId.SridLambert2008;

        private readonly IntegrationContext _integrationContext;

        public Importer(IntegrationContext integrationContext)
        {
            _integrationContext = integrationContext;
        }

        public async Task ExecuteAsync()
        {
            await InsertOrUpdateCurrentGeometries();
            await InsertOrUpdateGeometries2019();
        }

        private async Task InsertOrUpdateGeometries2019()
        {
            var vrbgNamespace = XNamespace.Get("https://geo.api.vlaanderen.be/VRBG2019");

            var wfsGetMunicipalityGeometry = "https://geo.api.vlaanderen.be/VRBG2019/wfs?service=WFS" +
                                             "&version=1.1.0" +
                                             "&request=GetFeature" +
                                             "&typeName=VRBG2019:Refgem" +
                                             "&maxFeatures=1" +
                                             "&srsName=EPSG:{0}" +
                                             "&CQL_FILTER=NISCODE=";

            using var httpClient = new HttpClient();

            foreach (var nisCode in await _integrationContext.MunicipalityLatestItems.Select(x => x.NisCode).ToListAsync())
            {
                using var response08 = await httpClient.GetAsync(string.Format(wfsGetMunicipalityGeometry, SrIdLambert08) + nisCode);
                using var response72 = await httpClient.GetAsync(string.Format(wfsGetMunicipalityGeometry, SrIdLambert72) + nisCode);

                var geometry08 = await ExtractGeometryFromResponse(response08, nisCode, SrIdLambert08, vrbgNamespace);
                var geometry72 = await ExtractGeometryFromResponse(response72, nisCode, SrIdLambert72, vrbgNamespace);

                if (geometry08 is null || geometry72 is null)
                    continue;

                if (!ValidateMunicipalityGeometry(geometry72, nisCode) || !ValidateMunicipalityGeometry(geometry08, nisCode))
                    continue;

                var municipalityGeometry = _integrationContext.MunicipalityGeometries2019.FirstOrDefault(x => x.NisCode == nisCode);

                if (municipalityGeometry is null)
                {
                    _integrationContext.MunicipalityGeometries2019.Add(new MunicipalityGeometry2019
                    {
                        NisCode = nisCode,
                        Geometry = geometry72,
                        GeometryLambert08 = geometry08
                    });
                }
                else
                {
                    if (municipalityGeometry.Geometry.EqualsTopologically(geometry72) && municipalityGeometry.GeometryLambert08.EqualsTopologically(geometry08))
                    {
                        continue;
                    }

                    municipalityGeometry.Geometry = geometry72;
                    municipalityGeometry.GeometryLambert08 = geometry08;
                }

                await _integrationContext.SaveChangesAsync();
                Log.Information($"Municipality {nisCode} geometry (2019) has been inserted/updated.");
            }
        }

        private async Task InsertOrUpdateCurrentGeometries()
        {
            var vrbgNamespace = XNamespace.Get("https://geo.api.vlaanderen.be/VRBG");

            var wfsGetMunicipalityGeometry = "https://geo.api.vlaanderen.be/VRBG/wfs?service=WFS" +
                                             "&version=1.1.0" +
                                             "&request=GetFeature" +
                                             "&typeName=VRBG:Refgem" +
                                             "&maxFeatures=1" +
                                             "&srsName=EPSG:{0}" +
                                             "&CQL_FILTER=NISCODE=";

            using var httpClient = new HttpClient();

            foreach (var nisCode in await _integrationContext.MunicipalityLatestItems.Select(x => x.NisCode).ToListAsync())
            {
                using var response08 = await httpClient.GetAsync(string.Format(wfsGetMunicipalityGeometry, SrIdLambert08) + nisCode);
                using var response72 = await httpClient.GetAsync(string.Format(wfsGetMunicipalityGeometry, SrIdLambert72) + nisCode);

                var geometry08 = await ExtractGeometryFromResponse(response08, nisCode, SrIdLambert08, vrbgNamespace);
                var geometry72 = await ExtractGeometryFromResponse(response72, nisCode, SrIdLambert72, vrbgNamespace);

                if (geometry08 is null || geometry72 is null)
                    continue;

                if (!ValidateMunicipalityGeometry(geometry72, nisCode) || !ValidateMunicipalityGeometry(geometry08, nisCode))
                    continue;

                var municipalityGeometry = _integrationContext.MunicipalityGeometries.FirstOrDefault(x => x.NisCode == nisCode);

                if (municipalityGeometry is null)
                {
                    _integrationContext.MunicipalityGeometries.Add(new MunicipalityGeometry
                    {
                        NisCode = nisCode,
                        Geometry = geometry72,
                        GeometryLambert08 = geometry08
                    });
                }
                else
                {
                    if (municipalityGeometry.Geometry.EqualsTopologically(geometry72) && municipalityGeometry.GeometryLambert08.EqualsTopologically(geometry08))
                    {
                        continue;
                    }

                    municipalityGeometry.Geometry = geometry72;
                    municipalityGeometry.GeometryLambert08 = geometry08;
                }

                await _integrationContext.SaveChangesAsync();
                Log.Information($"Municipality {nisCode} geometry has been inserted/updated.");
            }
        }

        private static bool ValidateMunicipalityGeometry(Geometry geometry, string nisCode)
        {
            var validOp = new IsValidOp(geometry)
            {
                IsSelfTouchingRingFormingHoleValid = true
            };

            if (!validOp.IsValid)
            {
                Log.Error($"Invalid Geometry for {nisCode}");
                return false;
            }

            return true;
        }

        private async Task<Geometry?> ExtractGeometryFromResponse(
            HttpResponseMessage response,
            string nisCode,
            int srid,
            XNamespace vrbgNamespace)
        {
            response.EnsureSuccessStatusCode(); // ok to crash at this point

            var stream = await response.Content.ReadAsStreamAsync();

            var data = XDocument.Load(stream);

            var shapeElement = data.Descendants(vrbgNamespace + "SHAPE").FirstOrDefault();

            if (shapeElement is null)
            {
                Log.Warning($"No geometry found for niscode: {nisCode}");
                return null;
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
            geometry.SRID = srid;
            return geometry;
        }
    }
}
