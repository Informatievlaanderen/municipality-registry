namespace MunicipalityRegistry.Tests.AggregateTests.WhenActivatingMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Exceptions;
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
        public void WithMunicipalityRetired_ThenMunicipalityHasInvalidStatusExceptionIsThrown()
        {
            var command = _fixture.Create<ActivateMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityWasRetired>())
                    .When(command)
                    .Throws(new MunicipalityHasInvalidStatusException()));
        }

        [Fact]
        public void WithMunicipalityCurrent_ThenNone()
        {
            var command = _fixture.Create<ActivateMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityBecameCurrent>())
                    .When(command)
                    .ThenNone());
        }

        [Fact]
        public void ThenMunicipalityBecameCurrent()
        {
            var command = _fixture.Create<ActivateMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>())
                    .When(command)
                    .Then(_municipalityId, new MunicipalityBecameCurrent(_municipalityId)));
        }

        [Fact]
        public void StateCheck()
        {
            // Arrange
            var municipalityBecameCurrent = _fixture.Create<MunicipalityBecameCurrent>();

            // Act
            var sut = Municipality.Municipality.Factory();
            sut.Initialize(new object[]
            {
                _fixture.Create<MunicipalityWasRegistered>(),
                municipalityBecameCurrent
            });

            // Assert
            sut.MunicipalityId.Should().Be(_municipalityId);
            sut.Status.Should().Be(MunicipalityStatus.Current);
        }
    }
}
