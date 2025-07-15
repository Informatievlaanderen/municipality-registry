namespace MunicipalityRegistry.Tests.AggregateTests.WhenRemovingMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using FluentAssertions;
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
            _fixture.Customize(new WithExtendedWkbGeometryPolygon());
            _fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = _fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void WithAlreadyRemovedMunicipality_ThenNone()
        {
            var command = _fixture.Create<RemoveMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityWasRemoved>())
                    .When(command)
                    .ThenNone());
        }

        [Fact]
        public void ThenMunicipalityWasRemoved()
        {
            var command = _fixture.Create<RemoveMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>())
                    .When(command)
                    .Then(_municipalityId, new MunicipalityWasRemoved(_municipalityId, _fixture.Create<NisCode>())));
        }

        [Fact]
        public void StateCheck()
        {
            var municipalityWasRemoved = _fixture.Create<MunicipalityWasRemoved>();

            var sut = Municipality.Municipality.Factory();
            sut.Initialize([
                _fixture.Create<MunicipalityWasRegistered>(),
                municipalityWasRemoved
            ]);

            sut.MunicipalityId.Should().Be(_municipalityId);
            sut.IsRemoved.Should().BeTrue();
        }
    }
}
