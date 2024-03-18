namespace MunicipalityRegistry.Tests.AutoFixture
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using global::AutoFixture.Dsl;
    using global::AutoFixture.Kernel;
    using MethodInvoker = global::AutoFixture.Kernel.MethodInvoker;

    public class WithFixedMunicipalityId : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Freeze<CrabMunicipalityId>();

            var deterministicId = fixture.Create<CrabMunicipalityId>().CreateDeterministicId();
            var municipalityId = new MunicipalityId(deterministicId);

            fixture.Register(() => municipalityId);

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(deterministicId),
                    new ParameterSpecification(
                        typeof(Guid),
                        "municipalityId")));
        }
    }

    public class WithFixedNisCode : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var nisCode = fixture.Create<NisCode>();
            fixture.Register(() => nisCode);

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(nisCode.ToString()),
                    new ParameterSpecification(
                        typeof(string),
                        "nisCode")));
        }
    }

    public class WithIntegerNisCode : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var nisCode = new NisCode(fixture.Create<int>().ToString());
            fixture.Register(() => nisCode);

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(nisCode.ToString()),
                    new ParameterSpecification(
                        typeof(string),
                        "nisCode")));
        }
    }

    public class WithWkbGeometry : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(fixture.Create<WkbGeometry>().ToString()),
                    new ParameterSpecification(
                        typeof(string),
                        "wkbGeometry")));

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder((int)SpatialReferenceSystemId.Lambert72),
                    new ParameterSpecification(
                        typeof(int),
                        "spatialReferenceSystemId")));
        }
    }

    public class WithFixedProvenance : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var timestamp = fixture.Create<CrabTimestamp>();
            var organisation = fixture.Create<CrabOrganisation>();
            var @operator = fixture.Create<CrabOperator>();
            var modification = fixture.Create<CrabModification>();

            fixture.Register(() => timestamp);
            fixture.Register(() => organisation);
            fixture.Register(() => @operator);
            fixture.Register(() => modification);
        }
    }

    public class WithProvenanceEventsUsePrivateConstructor : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            bool IsEventNamespace(Type t) => t.Namespace.EndsWith("Events");
            bool IsNotCompilerGenerated(MemberInfo t) => Attribute.GetCustomAttribute(t, typeof(CompilerGeneratedAttribute)) == null;

            var provenanceEventTypes = typeof(DomainAssemblyMarker).Assembly
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace != null && IsEventNamespace(t) && IsNotCompilerGenerated(t) && t.GetInterfaces().Any(x => x == typeof(ISetProvenance)));

            foreach (var allEventType in provenanceEventTypes)
            {
                var customizationComposerGenericType = typeof(ICustomizationComposer<>).MakeGenericType(allEventType);
                var specimenFactoryType = typeof(Func<,>).MakeGenericType(customizationComposerGenericType, typeof(ISpecimenBuilder));
                var fromFactoryMethod = GetType()
                    .GetMethod(nameof(FromFactory), BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(allEventType);

                var customizeMethod = typeof(Fixture).GetMethods().Single(m => m.Name == "Customize" && m.IsGenericMethod);
                var genericCustomizeMethod = customizeMethod.MakeGenericMethod(allEventType);
                var @delegate = Delegate.CreateDelegate(specimenFactoryType, this, fromFactoryMethod);
                genericCustomizeMethod.Invoke(fixture, new object[] {@delegate});
            }
        }

        private ISpecimenBuilder FromFactory<T>(IFactoryComposer<T> c) => c.FromFactory(new MethodInvoker(new PrivateGreedyConstructorQuery()));
    }
}
