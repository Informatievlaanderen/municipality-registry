namespace MunicipalityRegistry.Tests.Crab.GivenMuncipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Exceptions;
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
            _fixture.Customize(new WithFixedProvenance());
            _fixture.Customize(new WithProvenanceEventsUsePrivateConstructor());
            _fixture.Customize(new WithWkbGeometry());

            _municipalityId = _fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void WithNoNisCode()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.NisCode, (NisCode) null)
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)
                    .Throws(new NoNisCodeException("NisCode of a municipality cannot be empty.")));
        }

        [Fact]
        public void WithCorrectedNisCode()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?) null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Modification, CrabModification.Correction)
                .With(x => x.Geometry, (WkbGeometry) null)
                .With(x => x.NisCode, new NisCode(_fixture.Create<string>()))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityNisCodeWasCorrected = new MunicipalityNisCodeWasCorrected(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityNisCodeWasCorrected).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityNisCodeWasCorrected,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithFiniteLifeTimeAndMunicipalityIsCurrent()
        {
            var retirementDate = _fixture.Create<LocalDateTime>();
            var retirementInstant = retirementDate.ToCrabInstant();

            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Geometry, (WkbGeometry) null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), retirementDate))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityWasRetired = new MunicipalityWasRetired(_municipalityId, new RetirementDate(retirementInstant));
            ((ISetProvenance)municipalityWasRetired).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityWasRetired,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithFiniteLifeTimeAndCorrectionAndMunicipalityIsCurrent()
        {
            var retirementDate = _fixture.Create<LocalDateTime>();
            var retirementInstant = retirementDate.ToCrabInstant();

            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.Modification, CrabModification.Correction)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Geometry, (WkbGeometry) null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), retirementDate))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityWasCorrectedToRetired = new MunicipalityWasCorrectedToRetired(_municipalityId, new RetirementDate(retirementInstant));
            ((ISetProvenance)municipalityWasCorrectedToRetired).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityWasCorrectedToRetired,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithInfiniteLifeTimeAndMunicipalityIsRetired()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Geometry, (WkbGeometry) null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityBecameCurrent = new MunicipalityBecameCurrent(_municipalityId);
            ((ISetProvenance)municipalityBecameCurrent).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityWasRegistered>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityBecameCurrent,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithInfiniteLifeTimeAndCorrectionAndMunicipalityIsRetired()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Modification, CrabModification.Correction)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, (WkbGeometry) null)
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityWasCorrectedToCurrent = new MunicipalityWasCorrectedToCurrent(_municipalityId);
            ((ISetProvenance)municipalityWasCorrectedToCurrent).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityWasRetired>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityWasCorrectedToCurrent,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewFacilityLanguage()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?)null)
                .With(x => x.FacilityLanguage, CrabLanguage.German)
                .With(x => x.Geometry, (WkbGeometry)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityExistingFacilityLanguage = new MunicipalityFacilityLanguageWasAdded(_municipalityId, Language.French);
            ((ISetProvenance)municipalityExistingFacilityLanguage).SetProvenance(CreateProvenance(1));

            var municipalityFacilityLanguageWasRemoved = new MunicipalityFacilityLanguageWasRemoved(_municipalityId, Language.French);
            ((ISetProvenance)municipalityFacilityLanguageWasRemoved).SetProvenance(CreateProvenance(1));

            var municipalityFacilityLanguageWasAdded = new MunicipalityFacilityLanguageWasAdded(_municipalityId, Language.German);
            ((ISetProvenance)municipalityFacilityLanguageWasAdded).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalityExistingFacilityLanguage,
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityFacilityLanguageWasAdded,
                        municipalityFacilityLanguageWasRemoved,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewFacilityLanguageNull()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?)null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Geometry, (WkbGeometry)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityExistingFacilityLanguage = new MunicipalityFacilityLanguageWasAdded(_municipalityId, Language.French);
            ((ISetProvenance)municipalityExistingFacilityLanguage).SetProvenance(CreateProvenance(1));

            var municipalityFacilityLanguageWasRemoved = new MunicipalityFacilityLanguageWasRemoved(_municipalityId, Language.French);
            ((ISetProvenance)municipalityFacilityLanguageWasRemoved).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalityExistingFacilityLanguage,
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityFacilityLanguageWasRemoved,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewPrimaryLanguage()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.English)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Geometry, (WkbGeometry) null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityOfficialLanguageWasRemoved = new MunicipalityOfficialLanguageWasRemoved(_municipalityId, Language.Dutch);
            ((ISetProvenance)municipalityOfficialLanguageWasRemoved).SetProvenance(CreateProvenance(1));

            var municipalityOfficialLanguageWasAdded = new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.English);
            ((ISetProvenance)municipalityOfficialLanguageWasAdded).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityOfficialLanguageWasAdded,
                        municipalityOfficialLanguageWasRemoved,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewPrimaryLanguageAsNull()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, (CrabLanguage?) null)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Geometry, (WkbGeometry) null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityOfficialLanguageWasRemoved = new MunicipalityOfficialLanguageWasRemoved(_municipalityId, Language.Dutch);
            ((ISetProvenance)municipalityOfficialLanguageWasRemoved).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityOfficialLanguageWasRemoved,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewSecondaryLanguage()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, CrabLanguage.French)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, (WkbGeometry) null)
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityOfficialLanguageWasAdded = new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.French);
            ((ISetProvenance)municipalityOfficialLanguageWasAdded).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityOfficialLanguageWasAdded,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewSecondaryLanguageAsNull()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, (WkbGeometry) null)
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalitySecondOfficialLanguageWasRemoved = new MunicipalityOfficialLanguageWasRemoved(_municipalityId, Language.French);
            ((ISetProvenance)municipalitySecondOfficialLanguageWasRemoved).SetProvenance(CreateProvenance(1));

            var municipalitySecondOfficialLanguageWasAdded = new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.French);
            ((ISetProvenance)municipalitySecondOfficialLanguageWasAdded).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalitySecondOfficialLanguageWasAdded,
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalitySecondOfficialLanguageWasRemoved,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewGeometry()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, new WkbGeometry(GeometryHelpers.ExampleWkb))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var expectedMunicipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)expectedMunicipalityWasDrawn).SetProvenance(CreateProvenance(1));

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalityWasDrawn,
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        expectedMunicipalityWasDrawn,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewGeometryAndCorrection()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Modification, CrabModification.Correction)
                .With(x => x.Geometry, new WkbGeometry(GeometryHelpers.ExampleWkb))
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityGeometryWasCorrected = new MunicipalityGeometryWasCorrected(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)municipalityGeometryWasCorrected).SetProvenance(CreateProvenance(1));

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalityWasDrawn,
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityGeometryWasCorrected,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewGeometryAsNull()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, (WkbGeometry) null)
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityGeometryWasCleared = new MunicipalityGeometryWasCleared(_municipalityId);
            ((ISetProvenance)municipalityGeometryWasCleared).SetProvenance(CreateProvenance(1));

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                         municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalityWasDrawn,
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityGeometryWasCleared,
                        importMunicipalityFromCrab.ToLegacyEvent()));
        }

        [Fact]
        public void WithNewGeometryAsNullAndCorrection()
        {
            var importMunicipalityFromCrab = _fixture
                .Build<ImportMunicipalityFromCrab>()
                .With(x => x.PrimaryLanguage, CrabLanguage.Dutch)
                .With(x => x.Modification, CrabModification.Correction)
                .With(x => x.SecondaryLanguage, (CrabLanguage?) null)
                .With(x => x.FacilityLanguage, (CrabLanguage?)null)
                .With(x => x.Lifetime, new CrabLifetime(_fixture.Create<LocalDateTime>(), null))
                .With(x => x.Geometry, (WkbGeometry) null)
                .Create();

            var municipalityWasRegistered = new MunicipalityWasRegistered(_municipalityId, importMunicipalityFromCrab.NisCode);
            ((ISetProvenance)municipalityWasRegistered).SetProvenance(CreateProvenance(1));

            var municipalityGeometryWasCorrectedToCleared = new MunicipalityGeometryWasCorrectedToCleared(_municipalityId);
            ((ISetProvenance)municipalityGeometryWasCorrectedToCleared).SetProvenance(CreateProvenance(1));

            var municipalityWasDrawn = new MunicipalityWasDrawn(_municipalityId, new ExtendedWkbGeometry(GeometryHelpers.ExampleExtendedWkb));
            ((ISetProvenance)municipalityWasDrawn).SetProvenance(CreateProvenance(1));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        municipalityWasRegistered,
                        _fixture.Create<MunicipalityNisCodeWasDefined>(),
                        _fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                        municipalityWasDrawn,
                        _fixture.Create<MunicipalityBecameCurrent>())

                    .When(importMunicipalityFromCrab)

                    .Then(_municipalityId,
                        municipalityGeometryWasCorrectedToCleared,
                        importMunicipalityFromCrab.ToLegacyEvent()));
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
