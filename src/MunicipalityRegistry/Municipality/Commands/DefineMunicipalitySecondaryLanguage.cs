namespace MunicipalityRegistry.Municipality.Commands
{
    public sealed class DefineMunicipalitySecondaryLanguage
    {
        public NisCode NisCode { get; }

        public Language Language { get; }

        public DefineMunicipalitySecondaryLanguage(
            NisCode nisCode,
            Language language)
        {
            NisCode = nisCode;
            Language = language;
        }
    }
}
