namespace MunicipalityRegistry.Tests.AggregateTests.WhenRemovingMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenNoMunicipality : MunicipalityRegistryTest
    {
        private readonly Fixture _fixture;

        public GivenNoMunicipality(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
        }

        [Fact]
        public void ThenAggregateNotFoundExceptionIsThrown()
        {
            var command = _fixture.Create<RemoveMunicipality>();

            Assert(
                new Scenario()
                    .Given()
                    .When(command)
                    .Throws(new AggregateNotFoundException(command.MunicipalityId.ToString(), typeof(Municipality))));
        }
    }
}
