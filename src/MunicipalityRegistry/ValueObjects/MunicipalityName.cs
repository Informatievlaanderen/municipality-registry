namespace MunicipalityRegistry
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Exceptions;

    public sealed class MunicipalityName : ValueObject<MunicipalityName>
    {
        public string Name { get; }

        public Language Language { get; }

        public MunicipalityName(string name, Language language)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new NoNameException("Name of a municipality cannot be empty.");

            Name = name;
            Language = language;
        }

        protected override IEnumerable<object> Reflect()
        {
            yield return Name;
            yield return Language;
        }

        public override string ToString() => $"{Name} ({Language})";
    }
}
