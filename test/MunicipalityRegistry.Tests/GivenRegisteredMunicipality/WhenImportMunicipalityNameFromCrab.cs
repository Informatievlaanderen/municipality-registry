namespace MunicipalityRegistry.Tests.GivenRegisteredMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands.Crab;
    using Municipality.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenImportMunicipalityNameFromCrab : MunicipalityRegistryTest
    {
        private readonly Fixture _fixture;
        private readonly MunicipalityId _municipalityId;
        private readonly NisCode _nisCode = new NisCode("123");

        public WhenImportMunicipalityNameFromCrab(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _fixture.Customize(new WithFixedMunicipalityId());
            _fixture.Customize(new NodaTimeCustomization());

            _municipalityId = _fixture.Create<MunicipalityId>();
        }

        [Theory]
        [InlineData(CrabLanguage.Dutch, Language.Dutch)]
        [InlineData(CrabLanguage.French, Language.French)]
        [InlineData(CrabLanguage.German, Language.German)]
        [InlineData(CrabLanguage.English, Language.English)]
        public void WithName(CrabLanguage language, Language expectedLanguage)
        {
            var name = _fixture.Create<string>();
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.MunicipalityName, new CrabMunicipalityName(name, language))
                .Create();

            var provenance = new MunicipalityProvenanceFactory().CreateFrom(1, false, municipalityNameFromCrab.Timestamp, municipalityNameFromCrab.Modification, municipalityNameFromCrab.Operator, municipalityNameFromCrab.Organisation);

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(provenance);

            var municipalityWasNamed = new MunicipalityWasNamed(_municipalityId, new MunicipalityName(name, expectedLanguage));
            ((ISetProvenance)municipalityWasNamed).SetProvenance(provenance);

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered)

                    .When(municipalityNameFromCrab)

                    .Then(_municipalityId,
                        municipalityWasNamed,
                        municipalityNameFromCrab.ToLegacyEvent()));
        }

        [Theory]
        [InlineData(CrabLanguage.Dutch)]
        [InlineData(CrabLanguage.French)]
        [InlineData(CrabLanguage.German)]
        [InlineData(CrabLanguage.English)]
        public void WithNoName(CrabLanguage language)
        {
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.MunicipalityName, new CrabMunicipalityName(null, language))
                .Create();

            var provenance = new MunicipalityProvenanceFactory().CreateFrom(1, false, municipalityNameFromCrab.Timestamp, municipalityNameFromCrab.Modification, municipalityNameFromCrab.Operator, municipalityNameFromCrab.Organisation);

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(provenance);

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered)

                    .When(municipalityNameFromCrab)
                    .Throws(new NoNameException("Cannot give a municipality an empty name.")));
        }
    }
}
