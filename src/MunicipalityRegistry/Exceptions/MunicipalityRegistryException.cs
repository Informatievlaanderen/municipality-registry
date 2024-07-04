namespace MunicipalityRegistry.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public abstract class MunicipalityRegistryException : DomainException
    {
        protected MunicipalityRegistryException()
        { }

        protected MunicipalityRegistryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        protected MunicipalityRegistryException(string message)
            : base(message)
        { }

        protected MunicipalityRegistryException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
