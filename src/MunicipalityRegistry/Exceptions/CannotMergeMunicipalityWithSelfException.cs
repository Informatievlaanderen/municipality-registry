namespace MunicipalityRegistry.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class CannotMergeMunicipalityWithSelfException : MunicipalityRegistryException
    {
        public CannotMergeMunicipalityWithSelfException()
        { }

        private CannotMergeMunicipalityWithSelfException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public CannotMergeMunicipalityWithSelfException(string message)
            : base(message)
        { }

        public CannotMergeMunicipalityWithSelfException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
