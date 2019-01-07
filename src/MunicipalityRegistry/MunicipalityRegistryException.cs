namespace MunicipalityRegistry
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public abstract class MunicipalityRegistryException : DomainException
    {
        protected MunicipalityRegistryException() { }

        protected MunicipalityRegistryException(string message) : base(message) { }

        protected MunicipalityRegistryException(string message, Exception inner) : base(message, inner) { }
    }
}
