namespace MunicipalityRegistry.Exceptions
{
    public sealed class MunicipalityNameDoesNotExistException : MunicipalityRegistryException
    {
        public MunicipalityNameDoesNotExistException(Language language)
            : base($"Municipality name does not exist for language {language}")
        { }
    }
}
