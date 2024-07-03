namespace MunicipalityRegistry.Tests.Crab.GivenMuncipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using MunicipalityRegistry.Municipality;
    using MunicipalityRegistry.Municipality.Commands.Crab;
    using MunicipalityRegistry.Municipality.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenImportingMunicipalityName : MunicipalityRegistryTest
    {
        private readonly Fixture _fixture;
        private readonly MunicipalityId _municipalityId;
        private readonly NisCode _nisCode = new NisCode("123");

        public WhenImportingMunicipalityName(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _fixture.Customize(new WithFixedMunicipalityId());
            _fixture.Customize(new WithFixedProvenance());
            _fixture.Customize(new NodaTimeCustomization());
            _fixture.Customize(new WithProvenanceEventsUsePrivateConstructor());

            _municipalityId = _fixture.Create<MunicipalityId>();
        }

        [Theory]
        [InlineData(CrabLanguage.Dutch, Language.Dutch)]
        [InlineData(CrabLanguage.French, Language.French)]
        [InlineData(CrabLanguage.German, Language.German)]
        [InlineData(CrabLanguage.English, Language.English)]
        public void WithNewNameAndMuncipalityHasName(CrabLanguage language, Language expectedLanguage)
        {
            var newName = _fixture.Create<string>();
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.MunicipalityName, new CrabMunicipalityName(newName, language))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityWasNamed = new MunicipalityWasNamed(_municipalityId, new MunicipalityName(newName, expectedLanguage));
            ((ISetProvenance)municipalityWasNamed).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityWasNamed>())

                    .When(municipalityNameFromCrab)

                    .Then(_municipalityId,
                        municipalityWasNamed,
                        municipalityNameFromCrab.ToLegacyEvent()));
        }

        [Theory]
        [InlineData(CrabLanguage.Dutch, Language.Dutch)]
        [InlineData(CrabLanguage.French, Language.French)]
        [InlineData(CrabLanguage.German, Language.German)]
        [InlineData(CrabLanguage.English, Language.English)]
        public void WithCorrectedNameAndMuncipalityHasName(CrabLanguage language, Language expectedLanguage)
        {
            var newName = _fixture.Create<string>();
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.Modification, CrabModification.Correction)
                .With(x => x.MunicipalityName, new CrabMunicipalityName(newName, language))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityNameWasCorrected = new MunicipalityNameWasCorrected(_municipalityId, new MunicipalityName(newName, expectedLanguage));
            ((ISetProvenance)municipalityNameWasCorrected).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityWasNamed>())

                    .When(municipalityNameFromCrab)

                    .Then(_municipalityId,
                        municipalityNameWasCorrected,
                        municipalityNameFromCrab.ToLegacyEvent()));
        }

        [Theory]
        [InlineData(CrabLanguage.Dutch, Language.Dutch)]
        [InlineData(CrabLanguage.French, Language.French)]
        [InlineData(CrabLanguage.German, Language.German)]
        [InlineData(CrabLanguage.English, Language.English)]
        public void WithNewNameAsNullAndMuncipalityHasName(CrabLanguage language, Language expectedLanguage)
        {
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.MunicipalityName, new CrabMunicipalityName(null, language))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityWasNamed = new MunicipalityWasNamed(_municipalityId, new MunicipalityName(_fixture.Create<string>(), expectedLanguage));
            ((ISetProvenance)municipalityWasNamed).SetProvenance(CreateProvenance(1));

            var municipalityNameWasCleared = new MunicipalityNameWasCleared(_municipalityId, expectedLanguage);
            ((ISetProvenance)municipalityNameWasCleared).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        municipalityWasNamed)

                    .When(municipalityNameFromCrab)
                    .Throws(new NoNameException("Cannot give a municipality an empty name.")));
        }

        [Theory]
        [InlineData(CrabLanguage.Dutch, Language.Dutch)]
        [InlineData(CrabLanguage.French, Language.French)]
        [InlineData(CrabLanguage.German, Language.German)]
        [InlineData(CrabLanguage.English, Language.English)]
        public void WithNewNameAsNullAndCorrectionAndMuncipalityHasName(CrabLanguage language, Language expectedLanguage)
        {
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.Modification, CrabModification.Correction)
                .With(x => x.MunicipalityName, new CrabMunicipalityName(null, language))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityNameWasCorrectedToCleared = new MunicipalityNameWasCorrectedToCleared(_municipalityId, expectedLanguage);
            ((ISetProvenance)municipalityNameWasCorrectedToCleared).SetProvenance(CreateProvenance(1));

            var municipalityWasNamed = new MunicipalityWasNamed(_municipalityId, new MunicipalityName(_fixture.Create<string>(), expectedLanguage));
            ((ISetProvenance)municipalityWasNamed).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        municipalityWasNamed)

                    .When(municipalityNameFromCrab)

                    .Throws(new NoNameException("Cannot give a municipality an empty name.")));
        }

        [Theory]
        [InlineData(CrabLanguage.Dutch, Language.Dutch)]
        [InlineData(CrabLanguage.French, Language.French)]
        [InlineData(CrabLanguage.German, Language.German)]
        [InlineData(CrabLanguage.English, Language.English)]
        public void WithSameNameAndMuncipalityHasSameName(CrabLanguage language, Language expectedLanguage)
        {
            var name = _fixture.Create<string>();
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.MunicipalityName, new CrabMunicipalityName(name, language))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityWasNamed = new MunicipalityWasNamed(_municipalityId, new MunicipalityName(name, expectedLanguage));
            ((ISetProvenance)municipalityWasNamed).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        municipalityWasNamed)

                    .When(municipalityNameFromCrab)

                    .Then(_municipalityId,
                        municipalityNameFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNullLanguageAndMuncipalityHasName()
        {
            var name = _fixture.Create<string>();
            var municipalityNameFromCrab = _fixture
                .Build<ImportMunicipalityNameFromCrab>()
                .With(x => x.MunicipalityName, new CrabMunicipalityName(name, null))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _nisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityWasNamed>())

                    .When(municipalityNameFromCrab)

                    .Then(_municipalityId,
                        municipalityNameFromCrab.ToLegacyEvent()));
        }

        private Provenance CreateProvenance(int version)
        {
            return new MunicipalityCrabProvenanceFactory().CreateFrom(
                version,
                false,
                _fixture.Create<CrabTimestamp>(),
                _fixture.Create<CrabModification>(),
                _fixture.Create<CrabOperator>(),
                _fixture.Create<CrabOrganisation>());
        }
    }
}
