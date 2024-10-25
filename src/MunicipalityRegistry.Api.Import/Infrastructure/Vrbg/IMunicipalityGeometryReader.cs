namespace MunicipalityRegistry.Api.Import.Infrastructure.Vrbg
{
    using System.Threading.Tasks;
    using NetTopologySuite.Geometries;

    public interface IMunicipalityGeometryReader
    {
        Task<Geometry> GetGeometry(string nisCode);
    }
}
