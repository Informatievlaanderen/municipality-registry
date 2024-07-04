namespace MunicipalityRegistry.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class NoNameException : MunicipalityRegistryException
    {
        public NoNameException()
        { }

        private NoNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public NoNameException(string message)
            : base(message)
        { }

        public NoNameException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
