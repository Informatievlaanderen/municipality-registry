namespace MunicipalityRegistry
{
    using System;

    public class NoNisCodeException : MunicipalityRegistryException
    {
        public NoNisCodeException() { }

        public NoNisCodeException(string message) : base(message) { }

        public NoNisCodeException(string message, Exception inner) : base(message, inner) { }
    }
}
