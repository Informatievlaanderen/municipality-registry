namespace MunicipalityRegistry
{
    using System;
    using Be.Vlaanderen.Basisregisters.Crab;

    public enum Language
    {
        Dutch = 0,
        French = 1,
        German = 2,
        English = 3
    }

    public static class LanguageHelpers
    {
        public static Language ToLanguage(this CrabLanguage language)
        {
            switch (language)
            {
                case CrabLanguage.Dutch:
                    return Language.Dutch;

                case CrabLanguage.French:
                    return Language.French;

                case CrabLanguage.German:
                    return Language.German;

                case CrabLanguage.English:
                    return Language.English;

                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, $"Non existing language '{language}'.");
            }
        }
    }
}
