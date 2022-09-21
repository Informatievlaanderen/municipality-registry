namespace MunicipalityRegistry.Municipality.Commands
{
    public sealed class RegisterMunicipality
    {
        public MunicipalityId MunicipalityId { get; }

        public NisCode NisCode { get; }

        public Language? PrimaryLanguage { get; }

        public Language? SecondaryLanguage { get; }

        public RegisterMunicipality(
            MunicipalityId municipalityId,
            NisCode nisCode,
            Language? primaryLanguage,
            Language? secondaryLanguage)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            PrimaryLanguage = primaryLanguage;
            SecondaryLanguage = secondaryLanguage;
        }
    }
}
