namespace MunicipalityRegistry.Tests
{
    using Be.Vlaanderen.Basisregisters.GrAr.Common.NetTopology;
    using FluentAssertions;
    using Xunit;

    public class GeometryHelpersTests
    {
        [Fact]
        public void ExampleLambert08WkbCanBePromotedToTheExpectedExtendedWkb()
        {
            var sut = ExtendedWkbGeometry.CreateEWkb(GeometryHelpers.ExampleWkbLambert08, SystemReferenceId.SridLambert2008);

            sut.Should().BeEquivalentTo(new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkbLambert08));
        }

        [Fact]
        public void ExampleLambert08ExtendedWkbCanBeReadAsLambert2008Geometry()
        {
            var geometry = WKBReaderFactory.CreateForLambert2008().Read(GeometryHelpers.ExampleWkbLambert08);

            geometry.SRID.Should().Be(SystemReferenceId.SridLambert2008);
            geometry.Coordinates[0].X.Should().Be(641296.776584937470);
            geometry.Coordinates[0].Y.Should().Be(685195.288073283271);
            geometry.Coordinates[1].X.Should().Be(641292.7453139233);
            geometry.Coordinates[1].Y.Should().Be(685189.4559618587);
        }
    }
}


