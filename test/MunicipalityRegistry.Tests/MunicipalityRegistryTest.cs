namespace MunicipalityRegistry.Tests
{
    using System.Collections.Generic;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing.Comparers;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing.SqlStreamStore.Autofac;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Infrastructure.Modules;
    using KellermanSoftware.CompareNetObjects;
    using Microsoft.Extensions.Configuration;
    using Xunit.Abstractions;

    public abstract class MunicipalityRegistryTest : AutofacBasedTest
    {
        protected MunicipalityRegistryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        protected override void ConfigureCommandHandling(ContainerBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "ConnectionStrings:Events", "x" } })
                .Build();

            builder.RegisterModule(new CommandHandlingModule(configuration));
        }

        protected override void ConfigureEventHandling(ContainerBuilder builder)
        {
            var eventSerializerSettings = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();
            builder.RegisterModule(new EventHandlingModule(typeof(DomainAssemblyMarker).Assembly, eventSerializerSettings));
        }

        protected override IFactComparer CreateFactComparer()
        {
            var comparer = new CompareLogic();
            comparer.Config.MembersToIgnore.Add("Provenance");
            return new CompareNetObjectsBasedFactComparer(comparer);
        }
    }
}
