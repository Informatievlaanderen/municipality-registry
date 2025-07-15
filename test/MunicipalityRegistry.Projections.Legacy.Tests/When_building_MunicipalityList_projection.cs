namespace MunicipalityRegistry.Projections.Legacy.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Legacy;
    using Microsoft.EntityFrameworkCore;
    using Municipality.Events;
    using MunicipalityList;
    using NodaTime;
    using Xunit;
    using MunicipalityName = MunicipalityRegistry.MunicipalityName;
    using Reason = Be.Vlaanderen.Basisregisters.GrAr.Provenance.Reason;

    public class When_building_MunicipalityList_projection
    {
        private static ConcurrentDictionary<string, object> EmptyMetadata => new ConcurrentDictionary<string, object>();

        private readonly Provenance _provenance = new Provenance(
            Instant.FromDateTimeOffset(new DateTimeOffset(new DateTime(2019, 10, 23, 14, 25, 34))),
            Application.BPost,
            Reason.CentralManagementCrab,
            new Operator("test"),
            Modification.Update,
            Organisation.Aiv);

        [Fact]
        public async Task Given_no_messages_Then_list_is_empty()
        {
            var projection = new MunicipalityListProjections();
            var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);

            await new ConnectedProjectionScenario<LegacyContext>(resolver)
                .Given()
                .Verify(async context =>
                    await context.MunicipalityList.AnyAsync()
                        ? VerificationResult.Fail()
                        : VerificationResult.Pass())
                .Assert();
        }

        [Fact]
        public async Task Given_MunicipalityWasRegistered_Then_expected_item_is_added()
        {
            var projection = new MunicipalityListProjections();
            var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);

            var municipalityId = Guid.NewGuid();
            var nisCode = "123";
            var municipalityWasRegistered = new MunicipalityWasRegistered(new MunicipalityId(municipalityId), new NisCode(nisCode));
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(_provenance);

            await new ConnectedProjectionScenario<LegacyContext>(resolver)
                .Given(
                    new Envelope<MunicipalityWasRegistered>(new Envelope(
                        municipalityWasRegistered,
                        EmptyMetadata)))

                .Verify(async context =>
                {
                    var municipality = await context.MunicipalityList.FirstAsync(a => a.MunicipalityId == municipalityId);

                    municipality.MunicipalityId
                        .Should()
                        .Be(municipalityId);

                    municipality.NisCode
                        .Should()
                        .Be(nisCode);

                    return VerificationResult.Pass();
                })

                .Assert();
        }

        [Fact]
        public async Task Given_MunicipalityDefinedNisCode_Then_nisCode_should_be_changed()
        {
            var projection = new MunicipalityListProjections();
            var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);

            var crabMunicipalityId = new CrabMunicipalityId(1);
            var municipalityId = MunicipalityId.CreateFor(crabMunicipalityId);

            var nisCode = "456";
            var municipalityWasRegistered = new MunicipalityWasRegistered(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(_provenance);

            var municipalityNisCodeWasDefined = new MunicipalityNisCodeWasDefined(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityNisCodeWasDefined).SetProvenance(_provenance);

            await new ConnectedProjectionScenario<LegacyContext>(resolver)
                .Given(
                    new Envelope<MunicipalityWasRegistered>(new Envelope(
                        municipalityWasRegistered,
                        EmptyMetadata)),

                    new Envelope<MunicipalityNisCodeWasDefined>(new Envelope(
                        municipalityNisCodeWasDefined,
                        EmptyMetadata)))
                .Verify(async context =>
                {
                    var municipality = await context.MunicipalityList.FirstAsync(a => a.NisCode == nisCode);

                    municipality.MunicipalityId.Should().Be((Guid)municipalityId);
                    municipality.NisCode.Should().Be(nisCode);

                    return VerificationResult.Pass();
                })
                .Assert();
        }

        [Fact]
        public async Task Given_MunicipalityDefinedPrimaryLanguage_Then_expected_item_is_updated()
        {
            var projection = new MunicipalityListProjections();
            var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);

            var crabMunicipalityId = new CrabMunicipalityId(1);
            var municipalityId = MunicipalityId.CreateFor(crabMunicipalityId);

            var nisCode = "456";
            var municipalityWasRegistered = new MunicipalityWasRegistered(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(_provenance);

            var municipalityOfficialLanguageWasAdded = new MunicipalityOfficialLanguageWasAdded(municipalityId, Language.Dutch);
            ((ISetProvenance)municipalityOfficialLanguageWasAdded).SetProvenance(_provenance);

            await new ConnectedProjectionScenario<LegacyContext>(resolver)
                .Given(
                    new Envelope<MunicipalityWasRegistered>(new Envelope(
                        municipalityWasRegistered,
                        EmptyMetadata)),

                    new Envelope<MunicipalityOfficialLanguageWasAdded>(new Envelope(
                        municipalityOfficialLanguageWasAdded,
                        EmptyMetadata)))
                .Verify(async context =>
                {
                    var municipality = await context.MunicipalityList.FirstAsync(a => a.MunicipalityId == municipalityId);

                    municipality.MunicipalityId.Should().Be((Guid)municipalityId);
                    municipality.OfficialLanguages.Should().AllBeEquivalentTo(Language.Dutch);

                    return VerificationResult.Pass();
                })
                .Assert();
        }

        [Fact]
        public async Task Given_MunicipalityWasNamed_and_municipality_was_registered_Then_expected_item_is_updated()
        {
            var projection = new MunicipalityListProjections();
            var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);

            var crabMunicipalityId = new CrabMunicipalityId(1);
            var municipalityId = MunicipalityId.CreateFor(crabMunicipalityId);

            var nisCode = "456";
            var municipalityWasRegistered = new MunicipalityWasRegistered(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(_provenance);

            var municipalityWasNamed = new MunicipalityWasNamed(municipalityId, new MunicipalityName("test", Language.Dutch));
            ((ISetProvenance)municipalityWasNamed).SetProvenance(_provenance);

            await new ConnectedProjectionScenario<LegacyContext>(resolver)
                .Given(
                    new Envelope<MunicipalityWasRegistered>(new Envelope(
                        municipalityWasRegistered,
                        EmptyMetadata)),

                    new Envelope<MunicipalityWasNamed>(new Envelope(
                        municipalityWasNamed,
                        EmptyMetadata)))
                .Verify(async context =>
                {
                    var municipality = await context.MunicipalityList.FirstAsync(a => a.MunicipalityId == municipalityId);

                    municipality.MunicipalityId.Should().Be((Guid)municipalityId);
                    municipality.NameDutch.Should().Be("test");

                    return VerificationResult.Pass();
                })
                .Assert();
        }

        [Fact]
        public async Task Given_MunicipalityWasNamed_twice_and_municipality_was_registered_Then_expected_item_is_updated()
        {
            var projection = new MunicipalityListProjections();
            var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);

            var crabMunicipalityId = new CrabMunicipalityId(1);
            var municipalityId = MunicipalityId.CreateFor(crabMunicipalityId);

            var nisCode = "456";
            var municipalityWasRegistered = new MunicipalityWasRegistered(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(_provenance);

            var municipalityWasNamed = new MunicipalityWasNamed(municipalityId, new MunicipalityName("test", Language.Dutch));
            ((ISetProvenance)municipalityWasNamed).SetProvenance(_provenance);

            var wasNamed = new MunicipalityWasNamed(municipalityId, new MunicipalityName("test21", Language.French));
            ((ISetProvenance)wasNamed).SetProvenance(_provenance);

            await new ConnectedProjectionScenario<LegacyContext>(resolver)
                .Given(
                    new Envelope<MunicipalityWasRegistered>(new Envelope(
                        municipalityWasRegistered,
                        EmptyMetadata)),

                    new Envelope<MunicipalityWasNamed>(new Envelope(
                        municipalityWasNamed,
                        EmptyMetadata)),

                    new Envelope<MunicipalityWasNamed>(new Envelope(
                        wasNamed,
                        EmptyMetadata)))
                .Verify(async context =>
                {
                    var municipality = await context.MunicipalityList.FirstAsync(a => a.MunicipalityId == municipalityId);

                    municipality.MunicipalityId.Should().Be((Guid)municipalityId);
                    municipality.NameDutch.Should().Be("test");
                    municipality.NameFrench.Should().Be("test21");

                    return VerificationResult.Pass();
                })
                .Assert();
        }

        [Fact]
        public async Task Given_MunicipalityWasRemoved_Then_municipality_is_not_in_context()
        {
            var projection = new MunicipalityListProjections();
            var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);

            var crabMunicipalityId = new CrabMunicipalityId(1);
            var municipalityId = MunicipalityId.CreateFor(crabMunicipalityId);

            var nisCode = "456";
            var municipalityWasRegistered = new MunicipalityWasRegistered(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(_provenance);

            var municipalityNisCodeWasDefined = new MunicipalityNisCodeWasDefined(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityNisCodeWasDefined).SetProvenance(_provenance);

            var municipalityWasRemoved = new MunicipalityWasRemoved(municipalityId, new NisCode(nisCode));
            ((ISetProvenance)municipalityWasRemoved).SetProvenance(_provenance);

            await new ConnectedProjectionScenario<LegacyContext>(resolver)
                .Given(
                    new Envelope<MunicipalityWasRegistered>(new Envelope(
                        municipalityWasRegistered,
                        EmptyMetadata)),

                    new Envelope<MunicipalityNisCodeWasDefined>(new Envelope(
                        municipalityNisCodeWasDefined,
                        EmptyMetadata)),

                    new Envelope<MunicipalityWasRemoved>(new Envelope(
                        municipalityWasRemoved,
                        EmptyMetadata)))
                .Verify(async context =>
                {
                    var municipality = await context.MunicipalityList.FirstOrDefaultAsync(a => a.NisCode == nisCode);
                    municipality.Should().Be(null);

                    return VerificationResult.Pass();
                })
                .Assert();
        }
    }

    public static class ConnectedProjectionTestSpecificationExtensions
    {
        public static async Task Assert(this ConnectedProjectionTestSpecification<LegacyContext> specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            var options = new DbContextOptionsBuilder<LegacyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LegacyContext(options))
            {
                context.Database.EnsureCreated();

                foreach (var message in specification.Messages)
                {
                    await new ConnectedProjector<LegacyContext>(specification.Resolver)
                        .ProjectAsync(context, message);

                    await context.SaveChangesAsync();
                }

                var result = await specification.Verification(context, CancellationToken.None);
                if (result.Failed)
                    throw new AssertionFailedException(result.Message);
            }
        }
    }
}
