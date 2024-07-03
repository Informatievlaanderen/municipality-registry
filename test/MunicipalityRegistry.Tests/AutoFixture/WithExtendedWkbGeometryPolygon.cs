namespace MunicipalityRegistry.Tests.AutoFixture
{
    using global::AutoFixture;
    using global::AutoFixture.Kernel;

    public class WithExtendedWkbGeometryPolygon : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var extendedWkbGeometry = ExtendedWkbGeometry.CreateEWkb(GeometryHelpers.ExampleExtendedWkb);

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
