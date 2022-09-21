namespace MunicipalityRegistry.Municipality.Commands
{
    public sealed class DefineMunicipalityPrimaryLanguage
    {
        public NisCode NisCode { get; }

        public Language Language { get; }

        public DefineMunicipalityPrimaryLanguage(
            NisCode nisCode,
            Language language)
        {
            NisCode = nisCode;
            Language = language;
        }
    }
}
