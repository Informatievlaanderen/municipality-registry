namespace MunicipalityRegistry.Municipality.Commands
{
    public sealed class NameMunicipality
    {
        public NisCode NisCode { get; }

        public MunicipalityName Name { get; }

        public NameMunicipality(
            NisCode nisCode,
            MunicipalityName name)
        {
            NisCode = nisCode;
            Name = name;
        }
    }
}
