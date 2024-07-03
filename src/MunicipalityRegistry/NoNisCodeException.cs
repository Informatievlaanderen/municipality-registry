namespace MunicipalityRegistry
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class NoNisCodeException : MunicipalityRegistryException
    {
        public NoNisCodeException()
        { }

        private NoNisCodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public NoNisCodeException(string message)
            : base(message)
        { }

        public NoNisCodeException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
