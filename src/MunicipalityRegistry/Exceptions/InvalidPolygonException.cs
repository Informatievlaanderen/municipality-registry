namespace MunicipalityRegistry.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class InvalidPolygonException : MunicipalityRegistryException
    {
        public InvalidPolygonException()
        { }

        private InvalidPolygonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
