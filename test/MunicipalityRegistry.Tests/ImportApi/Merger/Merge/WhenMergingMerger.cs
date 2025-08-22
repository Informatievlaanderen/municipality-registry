namespace MunicipalityRegistry.Tests.ImportApi.Merger.Merge
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Import;
    using Api.Import.Merger;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using FluentValidation;
    using global::AutoFixture;
    using Municipality.Commands;
    using Municipality.Events;
    using Newtonsoft.Json;
    using Projections.Legacy.MunicipalityDetail;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class WhenMergingMerger : ImportApiTest
    {
        private readonly MergerController _controller;
        private readonly Fixture _fixture = new Fixture();
        private static JsonSerializerSettings JsonSerializerSettings = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();

        public WhenMergingMerger(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _controller = CreateMergerControllerWithUser<MergerController>();
        }

        [Fact]
        public void GivenNoMunicipalityMergers_ThenValidationErrorIsThrown()
        {
            var act = async () => await _controller.Merge(
                2000,
                CancellationToken.None);

            act
                .Should()
                .ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task GivenMunicipalityMergers_ThenMerge()
        {
            var mergerYear = 2025;

            var municipality1 = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10001"
            };
            var municipality2 = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10002"
            };
            var newMunicipality = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10000"
            };

            ImportContext.MunicipalityMergers.Add(
                new MunicipalityMerger(mergerYear,
                municipality1.MunicipalityId.Value,
                [municipality2.MunicipalityId.Value],
                newMunicipality.MunicipalityId.Value));
            ImportContext.MunicipalityMergers.Add(
                new MunicipalityMerger(mergerYear,
                municipality2.MunicipalityId.Value,
                [municipality1.MunicipalityId.Value],
                newMunicipality.MunicipalityId.Value));
            await ImportContext.SaveChangesAsync();

            LegacyContext.MunicipalityDetail.Add(municipality1);
            LegacyContext.MunicipalityDetail.Add(municipality2);
            LegacyContext.MunicipalityDetail.Add(newMunicipality);
            await LegacyContext.SaveChangesAsync();

            // setup domain
            DispatchArrangeCommand(new RegisterMunicipality(
                new MunicipalityId(municipality1.MunicipalityId.Value),
                new NisCode(municipality1.NisCode),
                [Language.Dutch],
                [],
                [new MunicipalityName(_fixture.Create<string>(), Language.Dutch)],
                new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb),
                _fixture.Create<Provenance>()));
            DispatchArrangeCommand(new RegisterMunicipality(
                new MunicipalityId(municipality2.MunicipalityId.Value),
                new NisCode(municipality2.NisCode),
                [Language.Dutch],
                [],
                [new MunicipalityName(_fixture.Create<string>(), Language.Dutch)],
                new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb),
                _fixture.Create<Provenance>()));
            DispatchArrangeCommand(new RegisterMunicipality(
                new MunicipalityId(newMunicipality.MunicipalityId.Value),
                new NisCode(newMunicipality.NisCode),
                [Language.Dutch],
                [],
                [new MunicipalityName(_fixture.Create<string>(), Language.Dutch)],
                new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb),
                _fixture.Create<Provenance>()));

            DispatchArrangeCommand(new ActivateMunicipality(
                new MunicipalityId(municipality1.MunicipalityId.Value),
                _fixture.Create<Provenance>()));
            DispatchArrangeCommand(new ActivateMunicipality(
                new MunicipalityId(municipality2.MunicipalityId.Value),
                _fixture.Create<Provenance>()));

            // Act
            await _controller.Merge(
                mergerYear,
                CancellationToken.None);

            var streamStore = Container.Resolve<IStreamStore>();

            var newMunicipalityMessages = await streamStore.ReadStreamBackwards(new StreamId(newMunicipality.MunicipalityId.Value.ToString("D")), 4, 1);
            newMunicipalityMessages.Messages.Length.Should().Be(1);
            newMunicipalityMessages.Messages[0].Type.Should().Be(nameof(MunicipalityBecameCurrent));
            var newMunicipalityBecameCurrent = JsonConvert.DeserializeObject<MunicipalityBecameCurrent>(await newMunicipalityMessages.Messages[0].GetJsonData(), JsonSerializerSettings);
            newMunicipalityBecameCurrent.Should().NotBeNull();

            var municipality1Messages = await streamStore.ReadStreamBackwards(new StreamId(municipality1.MunicipalityId.Value.ToString("D")), 5, 1);
            municipality1Messages.Messages.Length.Should().Be(1);
            municipality1Messages.Messages[0].Type.Should().Be(nameof(MunicipalityWasMerged));
            var municipality1WasMerged = JsonConvert.DeserializeObject<MunicipalityWasMerged>(await municipality1Messages.Messages[0].GetJsonData(), JsonSerializerSettings);
            municipality1WasMerged.Should().NotBeNull();

            var municipality2Messages = await streamStore.ReadStreamBackwards(new StreamId(municipality2.MunicipalityId.Value.ToString("D")), 5, 1);
            municipality2Messages.Messages.Length.Should().Be(1);
            municipality2Messages.Messages[0].Type.Should().Be(nameof(MunicipalityWasMerged));
            var municipality2WasMerged = JsonConvert.DeserializeObject<MunicipalityWasMerged>(await municipality2Messages.Messages[0].GetJsonData(), JsonSerializerSettings);
            municipality2WasMerged.Should().NotBeNull();
        }

        private void DispatchArrangeCommand<T>(T command) where T : IHasCommandProvenance
        {
            using var scope = Container.BeginLifetimeScope();
            var bus = scope.Resolve<ICommandHandlerResolver>();
            bus.Dispatch(command.CreateCommandId(), command).GetAwaiter().GetResult();
        }
    }
}
