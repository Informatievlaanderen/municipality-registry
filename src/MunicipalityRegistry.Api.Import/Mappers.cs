namespace MunicipalityRegistry.Api.Import
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;

    public static class Mappers
    {
        public static Language ToLanguage(this Taal taal)
        {
            return taal switch
            {
                Taal.NL => Language.Dutch,
                Taal.FR => Language.French,
                Taal.DE => Language.German,
                Taal.EN => Language.English,
                _ => throw new ArgumentOutOfRangeException(nameof(taal), taal, $"Non existing language '{taal}'.")
            };
        }
    }
}
