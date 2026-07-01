namespace MunicipalityRegistry.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class MunicipalityIsRemovedException : MunicipalityRegistryException
    {
        public MunicipalityIsRemovedException()
        { }

        private MunicipalityIsRemovedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public MunicipalityIsRemovedException(string message)
            : base(message)
        { }

        public MunicipalityIsRemovedException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
