namespace MunicipalityRegistry.Municipality.Commands
{
    public class DrawMunicipality
    {
        public NisCode NisCode { get; }

        public WkbGeometry WkbGeometry { get; }

        public DrawMunicipality(
            NisCode nisCode,
            WkbGeometry wkbGeometry)
        {
            NisCode = nisCode;
            WkbGeometry = wkbGeometry;
        }
    }
}
