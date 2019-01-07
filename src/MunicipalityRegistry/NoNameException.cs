namespace MunicipalityRegistry
{
    using System;

    public class NoNameException : MunicipalityRegistryException
    {
        public NoNameException() { }

        public NoNameException(string message) : base(message) { }

        public NoNameException(string message, Exception inner) : base(message, inner) { }
    }
}
