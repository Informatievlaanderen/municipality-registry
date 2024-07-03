namespace MunicipalityRegistry
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class DuplicateLanguageException : MunicipalityRegistryException
    {
        public DuplicateLanguageException()
        { }

        public DuplicateLanguageException(string message)
            : base(message)
        { }

        private DuplicateLanguageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
