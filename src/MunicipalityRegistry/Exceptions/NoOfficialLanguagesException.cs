namespace MunicipalityRegistry.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class NoOfficialLanguagesException : MunicipalityRegistryException
    {
        public NoOfficialLanguagesException()
        { }

        private NoOfficialLanguagesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public NoOfficialLanguagesException(string message)
            : base(message)
        { }

        public NoOfficialLanguagesException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
