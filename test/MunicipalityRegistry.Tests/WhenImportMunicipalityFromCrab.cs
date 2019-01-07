namespace MunicipalityRegistry.Tests
{
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using AutoFixture;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands.Crab;
    using Municipality.Events;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenImportMunicipalityFromCrab : MunicipalityRegistryTest
    {
        private readonly Fixture _fixture;
        private readonly ImportMunicipalityFromCrab _importMunicipalityFromCrab;
        private readonly MunicipalityId _municipalityId;

        public WhenImportMunicipalityFromCrab(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _fixture.Customize(new WithFixedProvenance());
            _fixture.Customize(new NodaTimeCustomization());

            _importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, CrabLanguage.French)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, new WkbGeometry(GeometryHelpers.ExampleWkb))
                .Create();

            _municipalityId = new MunicipalityId(_importMunicipalityFromCrab.MunicipalityId.CreateDeterministicId());

            _fixture
                .Build<MunicipalityId>()
                .FromFactory(() => _municipalityId)
                .Create();

            _fixture.Freeze<MunicipalityId>();
        }

        [Fact]
        public void ThenMunicipalityIsRegistered()
        {
            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, _importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityPrimaryLanguageWasDefined = new MunicipalityPrimaryLanguageWasDefined(_municipalityId, Language.Dutch);
            ((ISetProvenance)municipalityPrimaryLanguageWasDefined).SetProvenance(CreateProvenance(1));

            var municipalitySecondaryLanguageWasDefined = new MunicipalitySecondaryLanguageWasDefined(_municipalityId, Language.French);
            ((ISetProvenance)municipalitySecondaryLanguageWasDefined).SetProvenance(CreateProvenance(1));

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(CreateProvenance(1));

            var municipalityBecameCurrent = new MunicipalityBecameCurrent(_municipalityId);
            ((ISetProvenance)municipalityBecameCurrent).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .GivenNone()
                    .When(_importMunicipalityFromCrab)
                    .Then(_municipalityId,
                        municipalityWasRegistered,
                        municipalityPrimaryLanguageWasDefined,
                        municipalitySecondaryLanguageWasDefined,
                        municipalityWasDrawn,
                        municipalityBecameCurrent,
                        _importMunicipalityFromCrab.ToLegacyEvent()));
        }

        private Provenance CreateProvenance(int version)
        {
            return new MunicipalityProvenanceFactory().CreateFrom(
                version,
                false,
                _fixture.Create<CrabTimestamp>(),
                _fixture.Create<CrabModification>(),
                _fixture.Create<CrabOperator>(),
                _fixture.Create<CrabOrganisation>());
        }
    }
}
