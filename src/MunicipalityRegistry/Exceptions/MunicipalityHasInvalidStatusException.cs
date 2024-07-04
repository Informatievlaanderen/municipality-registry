namespace MunicipalityRegistry.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class MunicipalityHasInvalidStatusException : MunicipalityRegistryException
    {
        public MunicipalityHasInvalidStatusException()
        { }

        private MunicipalityHasInvalidStatusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public MunicipalityHasInvalidStatusException(string message)
            : base(message)
        { }

        public MunicipalityHasInvalidStatusException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
