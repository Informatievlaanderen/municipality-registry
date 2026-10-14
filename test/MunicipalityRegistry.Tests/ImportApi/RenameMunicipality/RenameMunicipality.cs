namespace MunicipalityRegistry.Tests.ImportApi.RenameMunicipality
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Import;
    using Api.Import.Requests;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Municipality.Commands;
    using Municipality.Events;
    using Newtonsoft.Json;
    using Projections.Legacy.MunicipalityDetail;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenRenamingMunicipality : MunicipalityRegistryTest
    {
        private readonly MunicipalityController _controller;
        private readonly Fixture _fixture = new Fixture();
        private readonly IIdempotentCommandHandler _commandHandler;
        private static JsonSerializerSettings JsonSerializerSettings = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();

        protected readonly FakeLegacyContext LegacyContext;

        public WhenRenamingMunicipality(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            LegacyContext = new FakeLegacyContextFactory().CreateDbContext();
            _commandHandler = Container.Resolve<IIdempotentCommandHandler>();
            _controller = CreateControllerWithUser<MunicipalityController>();
        }

        [Fact]
        public async Task GivenMunicipality_ThenMunicipalityNameWasCorrected()
        {
            var municipalityDetail = await ArrangeMunicipality();

            var result = await _controller.Rename(
                municipalityDetail.NisCode!,
                new RenameMunicipalityRequest { Naam = "Gent-Nieuw", Taal = Taal.NL },
                CancellationToken.None);

            result.Should().BeOfType<AcceptedResult>();

            var streamStore = Container.Resolve<IStreamStore>();
            var messages = await streamStore.ReadStreamBackwards(new StreamId(municipalityDetail.MunicipalityId!.Value.ToString("D")), StreamVersion.End, 1);
            messages.Messages.Length.Should().Be(1);
            messages.Messages[0].Type.Should().Be(nameof(MunicipalityNameWasCorrected));

            var nameWasCorrected = JsonConvert.DeserializeObject<MunicipalityNameWasCorrected>(await messages.Messages[0].GetJsonData(), JsonSerializerSettings);
            nameWasCorrected.Should().NotBeNull();
            nameWasCorrected!.MunicipalityId.Should().Be(municipalityDetail.MunicipalityId.Value);
            nameWasCorrected.Name.Should().Be("Gent-Nieuw");
            nameWasCorrected.Language.Should().Be(Language.Dutch);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task WithoutName_ThenBadRequest(string? naam)
        {
            var municipalityDetail = await ArrangeMunicipality();

            var result = await _controller.Rename(
                municipalityDetail.NisCode!,
                new RenameMunicipalityRequest { Naam = naam!, Taal = Taal.NL },
                CancellationToken.None);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private async Task<MunicipalityDetail> ArrangeMunicipality()
        {
            var municipalityDetail = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10001"
            };

            LegacyContext.MunicipalityDetail.Add(municipalityDetail);
            await LegacyContext.SaveChangesAsync();

            DispatchArrangeCommand(new RegisterMunicipality(
                new MunicipalityId(municipalityDetail.MunicipalityId.Value),
                new NisCode(municipalityDetail.NisCode),
                [Language.Dutch],
                [],
                [new MunicipalityName("Gent", Language.Dutch)],
                new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkbLambert08),
                _fixture.Create<Provenance>()));

            return municipalityDetail;
        }

        private void DispatchArrangeCommand<T>(T command) where T : IHasCommandProvenance
        {
            using var scope = Container.BeginLifetimeScope();
            var bus = scope.Resolve<ICommandHandlerResolver>();
            bus.Dispatch(command.CreateCommandId(), command).GetAwaiter().GetResult();
        }

        protected T CreateControllerWithUser<T>() where T : ApiController
        {
            var controller = Activator.CreateInstance(typeof(T), _commandHandler, LegacyContext) as T;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, "userId"),
                new Claim("name", "Username"),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            if (controller != null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claimsPrincipal };

                return controller;
            }

            throw new Exception("Could not find controller type");
        }
    }
}
