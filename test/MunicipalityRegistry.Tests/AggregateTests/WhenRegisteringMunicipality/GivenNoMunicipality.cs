namespace MunicipalityRegistry.Tests.AggregateTests.WhenRegisteringMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Exceptions;
    using FluentAssertions;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenNoMunicipality : MunicipalityRegistryTest
    {
        private readonly Fixture _fixture;
        private readonly MunicipalityId _municipalityId;

        public GivenNoMunicipality(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
            _fixture.Customize(new WithFixedNisCode());
            _fixture.Customize(new WithFixedMunicipalityId());
            _fixture.Customize(new WithExtendedWkbGeometryPolygon());

            _municipalityId = _fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void ThenMunicipalityWasRegistered()
        {
            // Arrange
            var command = _fixture.Create<RegisterMunicipality>()
                .WithOfficialLanguages([Language.Dutch, Language.French])
                .WithFacilityLanguages([Language.English, Language.German])
                .WithNames([
                    new(Language.Dutch, "Gent"),
                    new(Language.French, "Gand")
                ]);

            Assert(
                new Scenario()
                    .Given()
                    .When(command)
                    .Then(
                        new Fact(_municipalityId, new MunicipalityWasRegistered(_municipalityId, command.NisCode)),
                        new Fact(_municipalityId, new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.Dutch)),
                        new Fact(_municipalityId, new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.French)),
                        new Fact(_municipalityId, new MunicipalityFacilityLanguageWasAdded(_municipalityId, Language.English)),
                        new Fact(_municipalityId, new MunicipalityFacilityLanguageWasAdded(_municipalityId, Language.German)),
                        new Fact(_municipalityId, new MunicipalityWasNamed(_municipalityId, new MunicipalityName("Gent", Language.Dutch))),
                        new Fact(_municipalityId, new MunicipalityWasNamed(_municipalityId, new MunicipalityName("Gand", Language.French))),
                        new Fact(_municipalityId, new MunicipalityWasDrawn(_municipalityId, command.Geometry))
                    ));
        }

        [Fact]
        public void WithNoOfficialLanguages_ThenThrowsNoOfficialLanguagesException()
        {
            var command = _fixture.Create<RegisterMunicipality>()
                .WithOfficialLanguages([]);

            Assert(
                new Scenario()
                    .Given()
                    .When(command)
                    .Throws(new NoOfficialLanguagesException()));
        }

        [Fact]
        public void WithNoNames_ThenThrowsNoNameException()
        {
            var command = _fixture.Create<RegisterMunicipality>()
                .WithNames([]);

            Assert(
                new Scenario()
                    .Given()
                    .When(command)
                    .Throws(new NoNameException("At least one name is required.")));
        }

        [Fact]
        public void WithDuplicateLanguages_ThenThrowsDuplicateLanguageException()
        {
            var command = _fixture.Create<RegisterMunicipality>()
                .WithNames([
                    new(Language.Dutch, "Gent"),
                    new(Language.Dutch, "Gent2")
                ]);

            Assert(
                new Scenario()
                    .Given()
                    .When(command)
                    .Throws(new DuplicateLanguageException("Cannot give a municipality multiple names for the same language: Dutch")));
        }

        [Fact]
        public void WithInvalidGeometry_ThenThrowsInvalidPolygonException()
        {
            var command = _fixture.Create<RegisterMunicipality>()
                    .WithOfficialLanguages([Language.Dutch, Language.French])
                    .WithFacilityLanguages([Language.English, Language.German])
                    .WithNames([
                        new(Language.Dutch, "Gent"),
                        new(Language.French, "Gand")
                    ])
                    .WithGeometry(GeometryHelpers.InValidGmlPolygon.ToExtendedWkbGeometry());

            Assert(
                new Scenario()
                    .Given()
                    .When(command)
                    .Throws(new InvalidPolygonException()));
        }

        [Fact]
        public void StateCheck()
        {
            // Arrange
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityOfficialLanguageWasAdded = _fixture.Create<MunicipalityOfficialLanguageWasAdded>();
            var municipalityFacilityLanguageWasAdded = _fixture.Create<MunicipalityFacilityLanguageWasAdded>();
            var municipalityWasNamed = _fixture.Create<MunicipalityWasNamed>();
            var municipalityWasDrawn = _fixture.Create<MunicipalityWasDrawn>();

            // Act
            var sut = Municipality.Factory();
            sut.Initialize(new object[]
            {
                municipalityWasRegistered,
                municipalityOfficialLanguageWasAdded,
                municipalityFacilityLanguageWasAdded,
                municipalityWasNamed,
                municipalityWasDrawn
            });

            // Assert
            sut.MunicipalityId.Should().Be(_municipalityId);
            sut.NisCode.Should().Be(new NisCode(municipalityWasRegistered.NisCode));
            sut.Status.Should().Be(MunicipalityStatus.Proposed);
            sut.OfficialLanguages.Should().BeEquivalentTo(new[] { municipalityOfficialLanguageWasAdded.Language });
            sut.FacilitiesLanguages.Should().BeEquivalentTo(new[] { municipalityFacilityLanguageWasAdded.Language });
            sut.Names.Should().BeEquivalentTo(new[] { new MunicipalityName(municipalityWasNamed.Name, municipalityWasNamed.Language) });
            sut.Geometry.Should().BeEquivalentTo(new ExtendedWkbGeometry(municipalityWasDrawn.ExtendedWkbGeometry));
        }
    }
}
