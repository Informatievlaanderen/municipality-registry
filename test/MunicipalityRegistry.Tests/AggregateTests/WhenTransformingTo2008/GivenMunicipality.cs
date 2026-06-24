namespace MunicipalityRegistry.Tests.AggregateTests.WhenTransformingTo2008
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Exceptions;
    using FluentAssertions;
    using global::AutoFixture;
    using Municipality;
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
        public void WithMunicipalityRemoved_ThenMunicipalityIsRemovedExceptionIsThrown()
        {
            var command = _fixture.Create<TransformToLambert2008>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityWasRemoved>())
                    .When(command)
                    .Throws(new MunicipalityIsRemovedException()));
        }

        [Fact]
        public void WithMunicipalityAlreadyIn2008AndGeometryIsTheSame_ThenNone()
        {
            var command = _fixture.Create<TransformToLambert2008>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityBecameCurrent>(),
                        _fixture.Create<MunicipalityGeometryCrsWasChanged>())
                    .When(command)
                    .ThenNone());
        }

        [Fact]
        public void WithMunicipalityLambert1972_ThenMunicipalityGeometryCrsWasChanged()
        {
            var command = _fixture.Create<TransformToLambert2008>();
            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb1972));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(_fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityBecameCurrent>(),
                        municipalityWasDrawn)
                    .When(command)
                    .Then(_municipalityId,
                        new MunicipalityGeometryCrsWasChanged(_municipalityId,
                            new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkbLambert08))));
        }

        [Fact]
        public void StateCheck()
        {
            // Arrange
            var municipalityBecameCurrent = _fixture.Create<MunicipalityBecameCurrent>();
            var municipalityGeometryCrsWasChanged = _fixture.Create<MunicipalityGeometryCrsWasChanged>();

            // Act
            var sut = Municipality.Factory();
            sut.Initialize(new object[]
            {
                _fixture.Create<MunicipalityWasRegistered>(),
                municipalityBecameCurrent,
                municipalityGeometryCrsWasChanged
            });

            // Assert
            sut.MunicipalityId.Should().Be(_municipalityId);
            sut.Status.Should().Be(MunicipalityStatus.Current);
            sut.Geometry.Should().BeEquivalentTo(new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkbLambert08));
        }
    }
}
