namespace MunicipalityRegistry.Tests.Crab.GivenRegisteredMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using MunicipalityRegistry.Municipality;
    using MunicipalityRegistry.Municipality.Commands.Crab;
    using MunicipalityRegistry.Municipality.Events;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenImportMunicipalityFromCrab : MunicipalityRegistryTest
    {
        private readonly Fixture _fixture;
        private readonly MunicipalityId _municipalityId;

        public WhenImportMunicipalityFromCrab(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _fixture.Customize(new WithFixedMunicipalityId());
            _fixture.Customize(new WithFixedNisCode());
            _fixture.Customize(new NodaTimeCustomization());
            _fixture.Customize(new WithProvenanceEventsUsePrivateConstructor());
            _fixture.Customize(new WithWkbGeometry());

            _municipalityId = _fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void With2LanguagesAndInfiniteLifeTime()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, CrabLanguage.French)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, new WkbGeometry(GeometryHelpers.ExampleWkb))
                .Create();

            var provenance = new MunicipalityCrabProvenanceFactory().CreateFrom(1, false, importMunicipalityFromCrab.Timestamp, importMunicipalityFromCrab.Modification, importMunicipalityFromCrab.Operator, importMunicipalityFromCrab.Organisation);

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(provenance);

            var municipalityBecameCurrent = new MunicipalityBecameCurrent(_municipalityId);
            ((ISetProvenance)municipalityBecameCurrent).SetProvenance(provenance);

            var municipalityOfficialLanguageWasAdded = new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.Dutch);
            ((ISetProvenance)municipalityOfficialLanguageWasAdded).SetProvenance(provenance);

            var municipalitySecondOfficialLanguageWasAdded = new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.French);
            ((ISetProvenance)municipalitySecondOfficialLanguageWasAdded).SetProvenance(provenance);

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(provenance);

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered)

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityOfficialLanguageWasAdded,
                        municipalitySecondOfficialLanguageWasAdded,
                        municipalityWasDrawn,
                        municipalityBecameCurrent,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void With1LanguageAndFiniteLifeTime()
        {
            var retirementDate = _fixture.Create<LocalDateTime>();
            var retirementInstant = retirementDate.ToCrabInstant();

            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), retirementDate))
                .With(x => x.Geometry, new WkbGeometry(GeometryHelpers.ExampleWkb))
                .Create();

            var provenance = new MunicipalityCrabProvenanceFactory().CreateFrom(1, false, importMunicipalityFromCrab.Timestamp, importMunicipalityFromCrab.Modification, importMunicipalityFromCrab.Operator, importMunicipalityFromCrab.Organisation);

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(provenance);

            var municipalityPrimaryLanguageWasDefined = new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.Dutch);
            ((ISetProvenance)municipalityPrimaryLanguageWasDefined).SetProvenance(provenance);

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(provenance);

            var municipalityWasRetired = new MunicipalityWasRetired(_municipalityId, new RetirementDate(retirementInstant));
            ((ISetProvenance)municipalityWasRetired).SetProvenance(provenance);

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered)

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityPrimaryLanguageWasDefined,
                        municipalityWasDrawn,
                        municipalityWasRetired,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void With2LanguagesAndInfiniteLifeTimeAndGivenNothingChanged()
        {
            var retirementDate = _fixture.Create<LocalDateTime>();

            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Geometry, new WkbGeometry(GeometryHelpers.ExampleWkb))
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), retirementDate))
                .Create();

            var provenance = new MunicipalityCrabProvenanceFactory().CreateFrom(1, false, importMunicipalityFromCrab.Timestamp, importMunicipalityFromCrab.Modification, importMunicipalityFromCrab.Operator, importMunicipalityFromCrab.Organisation);

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(provenance);

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(provenance);

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalityWasDrawn,
                        _fixture.Create<MunicipalityWasRetired>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }
    }
}
