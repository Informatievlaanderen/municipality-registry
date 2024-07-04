namespace MunicipalityRegistry.Tests.AggregateTests.WhenMergingMunicipality
{
    using System;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Exceptions;
    using FluentAssertions;
    using global::AutoFixture;
    using Municipality.Commands;
    using Municipality.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipality : MunicipalityRegistryTest
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
        public void WithMunicipalityRetired_ThenMunicipalityHasInvalidStatusExceptionIsThrown()
        {
            var command = _fixture.Create<MergeMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityWasRetired>())
                    .When(command)
                    .Throws(new MunicipalityHasInvalidStatusException()));
        }

        [Fact]
        public void WithMunicipalityProposed_ThenMunicipalityHasInvalidStatusExceptionIsThrown()
        {
            var command = _fixture.Create<MergeMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>())
                    .When(command)
                    .Throws(new MunicipalityHasInvalidStatusException()));
        }

        [Fact]
        public void WithMunicipalityIsNewMunicipality_ThenCannotMergeMunicipalityWithSelfExceptionIsThrown()
        {
            var command = _fixture.Create<MergeMunicipality>();

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityBecameCurrent>())
                    .When(command)
                    .Throws(new CannotMergeMunicipalityWithSelfException()));
        }

        [Fact]
        public void ThenMunicipalityWasMerged()
        {
            var command = _fixture.Create<MergeMunicipality>()
                .WithMunicipalityIdsToMerge([
                    new MunicipalityId(_fixture.Create<Guid>()),
                    new MunicipalityId(_fixture.Create<Guid>())
                ])
                .WithNisCodesToMerge([
                    new NisCode(_fixture.Create<string>()),
                    new NisCode(_fixture.Create<string>())
                ])
                .WithNewMunicipalityId(new MunicipalityId(_fixture.Create<Guid>()))
                .WithNewNisCode(new NisCode(_fixture.Create<string>()));

            Assert(
                new Scenario()
                    .Given(_municipalityId,
                        _fixture.Create<MunicipalityWasRegistered>(),
                        _fixture.Create<MunicipalityBecameCurrent>())
                    .When(command)
                    .Then(
                        new Fact(_municipalityId, new MunicipalityWasMerged(
                            _municipalityId,
                            _fixture.Create<NisCode>(),
                            command.MunicipalityIdsToMergeWithWith,
                            command.NisCodesToMergeWith,
                            command.NewMunicipalityId,
                            command.NewNisCode))
                    ));
        }

        [Fact]
        public void StateCheck()
        {
            // Arrange
            var municipalityWasMerged = _fixture.Create<MunicipalityWasMerged>();

            // Act
            var sut = Municipality.Municipality.Factory();
            sut.Initialize(new object[]
            {
                _fixture.Create<MunicipalityWasRegistered>(),
                _fixture.Create<MunicipalityBecameCurrent>(),
                municipalityWasMerged
            });

            // Assert
            sut.MunicipalityId.Should().Be(_municipalityId);
            sut.Status.Should().Be(MunicipalityStatus.Retired);
        }
    }
}
