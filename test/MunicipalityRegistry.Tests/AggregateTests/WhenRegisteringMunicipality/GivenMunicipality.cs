namespace MunicipalityRegistry.Tests.AggregateTests.WhenRegisteringMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using Municipality.Commands;
    using Municipality.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipality : MunicipalityRegistryTest
    {
        private readonly Fixture _fixture;
        private readonly MunicipalityId _municipalityId;

        public GivenMunicipality(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
            _fixture.Customize(new WithFixedNisCode());
            _fixture.Customize(new WithFixedMunicipalityId());

            _municipalityId = _fixture.Create<MunicipalityId>();

        }

        [Fact]
        public void ThenAggregateSourceExceptionIsThrown()
        {
            // Arrange
            var command = _fixture.Create<RegisterMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>())
                    .When(command)
                    .Throws(new AggregateSourceException($"Municipality with id {_municipalityId} already exists")));
        }
    }
}
