namespace MunicipalityRegistry.Tests.AggregateTests.WhenChangingCrsOfMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.NetTopology;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using FluentAssertions;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using NetTopologySuite.IO;
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
        public void ThenGeometryCrsWasChanged()
        {
            var drawn = new MunicipalityWasDrawn(
                _municipalityId,
                new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)drawn).SetProvenance(_fixture.Create<Provenance>());
            var command = _fixture.Create<TransformToLambert2008>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityBecameCurrent>(),
                        drawn)
                    .When(command)
                    .Then(_municipalityId,
                        new MunicipalityGeometryCrsWasChanged(_municipalityId, command.Geometry)));
        }

        [Fact]
        public void WithMunicipalityAlreadyIn2008AndGeometryIsTheSame_ThenNone()
        {
            var command = _fixture.Create<ActivateMunicipality>();

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
        public void StateCheck()
        {
            // Arrange
            var drawn = new MunicipalityWasDrawn(
                _municipalityId,
                new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)drawn).SetProvenance(_fixture.Create<Provenance>());

            var crsChanged = new MunicipalityGeometryCrsWasChanged(
                _municipalityId,
                new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkbLambert08));
            ((ISetProvenance)crsChanged).SetProvenance(_fixture.Create<Provenance>());

            // Act
            var sut = Municipality.Factory();
            sut.Initialize(new object[]
            {
                _fixture.Create<MunicipalityWasRegistered>(),
                _fixture.Create<MunicipalityBecameCurrent>(),
                drawn,
                crsChanged
            });

            // Assert
            sut.MunicipalityId.Should().Be(_municipalityId);
            sut.Geometry.Should().NotBeNull();
            sut.Geometry!.ToString().Should().Be(GeometryHelpers.ExampleExtendedWkbLambert08.ToHexString());

            var wkbReader = WKBReaderFactory.CreateForLambert2008();
            var geometry = wkbReader.Read(sut.Geometry);

            geometry.Should().NotBeNull();
            geometry.SRID.Should().Be(SystemReferenceId.SridLambert2008);
        }
    }
}
