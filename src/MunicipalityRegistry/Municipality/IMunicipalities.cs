namespace MunicipalityRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public interface IMunicipalities : IAsyncRepository<Municipality, MunicipalityId> { }
}
