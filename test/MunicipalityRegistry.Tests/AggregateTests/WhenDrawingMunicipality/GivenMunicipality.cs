namespace MunicipalityRegistry.Tests.AggregateTests.WhenDrawingMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Exceptions;
    using global::AutoFixture;
    using Municipality.Commands;
    using Municipality.Events;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class GivenMunicipality : MunicipalityRegistryTest
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
        public void WithNoGeometry_ThenMunicipalityIsDrawn()
        {
            var command = _fixture.Create<DrawMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>())
                    .When(command)
                    .Then(new Fact(_municipalityId,
                        new MunicipalityWasDrawn(
                            _municipalityId,
                            command.Geometry))));
        }

        [Fact]
        public void WithNoChangeInGeometry_ThenNone()
        {
            var command = _fixture.Create<DrawMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityWasDrawn>())
                    .When(command)
                    .ThenNone());
        }

        [Fact]
        public void WithGeometryPresent_ThenMunicipalityGeometryWasCorrected()
        {
            var command = new DrawMunicipality(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.OtherExampleExtendedWkb), _fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityWasDrawn>())
                    .When(command)
                    .Then(new Fact(_municipalityId,
                        new MunicipalityGeometryWasCorrected(
                            _municipalityId,
                            command.Geometry))));
        }

        [Fact]
        public void WithInvalidGeometry_ThenThrowsInvalidPolygonException()
        {
            var command = new DrawMunicipality(_municipalityId, GeometryHelpers.InValidGmlPolygon.ToExtendedWkbGeometry(), _fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityWasDrawn>())
                    .When(command)
                    .Throws(new InvalidPolygonException()));
        }
    }
}
