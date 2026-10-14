namespace MunicipalityRegistry.Tests.AggregateTests.WhenRenamingMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Exceptions;
    using FluentAssertions;
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
        public void WithExistingNameInLanguage_ThenMunicipalityNameWasCorrected()
        {
            var command = new RenameMunicipality(
                _municipalityId,
                new MunicipalityName("Gent-Nieuw", Language.Dutch),
                _fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        Named("Gent", Language.Dutch))
                    .When(command)
                    .Then(new Fact(_municipalityId,
                        new MunicipalityNameWasCorrected(
                            _municipalityId,
                            command.NewName))));
        }

        [Fact]
        public void WithSameName_ThenNone()
        {
            var command = new RenameMunicipality(
                _municipalityId,
                new MunicipalityName("Gent", Language.Dutch),
                _fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        Named("Gent", Language.Dutch))
                    .When(command)
                    .ThenNone());
        }

        [Fact]
        public void WithNoNameInLanguage_ThenThrowsMunicipalityNameDoesNotExistException()
        {
            var command = new RenameMunicipality(
                _municipalityId,
                new MunicipalityName("Gand", Language.French),
                _fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        Named("Gent", Language.Dutch))
                    .When(command)
                    .Throws(new MunicipalityNameDoesNotExistException(Language.French)));
        }

        [Fact]
        public void WithClearedNameInLanguage_ThenThrowsMunicipalityNameDoesNotExistException()
        {
            var command = new RenameMunicipality(
                _municipalityId,
                new MunicipalityName("Gent-Nieuw", Language.Dutch),
                _fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        Named("Gent", Language.Dutch),
                        NameCleared(Language.Dutch))
                    .When(command)
                    .Throws(new MunicipalityNameDoesNotExistException(Language.Dutch)));
        }

        [Fact]
        public void WithRemovedMunicipality_ThenThrowsMunicipalityIsRemovedException()
        {
            var command = new RenameMunicipality(
                _municipalityId,
                new MunicipalityName("Gent-Nieuw", Language.Dutch),
                _fixture.Create<Provenance>());

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        Named("Gent", Language.Dutch),
                        _fixture.Create<MunicipalityWasRemoved>())
                    .When(command)
                    .Throws(new MunicipalityIsRemovedException()));
        }

        [Fact]
        public void StateCheck()
        {
            var sut = Municipality.Municipality.Factory();
            sut.Initialize([
                _fixture.Create<MunicipalityWasRegistered>(),
                Named("Gent", Language.Dutch),
                Named("Gand", Language.French),
                NameCorrected("Gent-Nieuw", Language.Dutch)
            ]);

            sut.Names.Should().BeEquivalentTo([
                new MunicipalityName("Gent-Nieuw", Language.Dutch),
                new MunicipalityName("Gand", Language.French)
            ]);
        }

        private MunicipalityWasNamed Named(string name, Language language)
            => WithProvenance(new MunicipalityWasNamed(_municipalityId, new MunicipalityName(name, language)));

        private MunicipalityNameWasCorrected NameCorrected(string name, Language language)
            => WithProvenance(new MunicipalityNameWasCorrected(_municipalityId, new MunicipalityName(name, language)));

        private MunicipalityNameWasCleared NameCleared(Language language)
            => WithProvenance(new MunicipalityNameWasCleared(_municipalityId, language));

        private T WithProvenance<T>(T @event) where T : ISetProvenance
        {
            @event.SetProvenance(_fixture.Create<Provenance>());
            return @event;
        }
    }
}
