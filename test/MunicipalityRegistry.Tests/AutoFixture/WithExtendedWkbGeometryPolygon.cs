namespace MunicipalityRegistry.Tests.AutoFixture
{
    using Be.Vlaanderen.Basisregisters.GrAr.Common.NetTopology;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;

    public class WithExtendedWkbGeometryPolygon : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var extendedWkbGeometry = ExtendedWkbGeometry.CreateEWkb(GeometryHelpers.ExampleExtendedWkb, SystemReferenceId.SridLambert72);

            fixture.Customize<ExtendedWkbGeometry>(c => c.FromFactory(
                () => extendedWkbGeometry));

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(extendedWkbGeometry.ToString()),
                    new ParameterSpecification(
                        typeof(string),
                        "extendedWkbGeometry")));
        }
    }
}
