namespace MunicipalityRegistry.Tests.Integration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Pipes;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using FluentAssertions;
    using global::AutoFixture;
    using Microsoft.Extensions.Options;
    using Municipality.Events;
    using Projections.Integration;
    using Projections.Integration.Infrastructure;
    using Xunit;

    public class MunicipalityVersionTests : IntegrationProjectionTest<MunicipalityVersionProjections>
    {
        private const string Namespace = "https://data.vlaanderen.be/id/gemeente";
        private readonly Fixture _fixture;

        public MunicipalityVersionTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
            _fixture.Customize(new WithFixedMunicipalityId());
            _fixture.Customize(new WithIntegerNisCode());
        }

        [Fact]
        public async Task WhenMunicipalityWasRegistered()
        {
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var position = _fixture.Create<long>();

            var metadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, metadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(position);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.NisCode.Should().Be(municipalityWasRegistered.NisCode);
                    expectedVersion.Namespace.Should().Be(Namespace);
                    expectedVersion.PuriId.Should().Be($"{Namespace}/{municipalityWasRegistered.NisCode}");
                    expectedVersion.VersionTimestamp.Should().Be(municipalityWasRegistered.Provenance.Timestamp);
                    expectedVersion.CreatedOnTimestamp.Should().Be(municipalityWasRegistered.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityNisCodeWasDefined()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityNisCodeWasDefined = _fixture.Create<MunicipalityNisCodeWasDefined>();
            var eventPosition = position + 1;
            var municipalityNisCodeWasDefinedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNisCodeWasDefined>(new Envelope(municipalityNisCodeWasDefined, municipalityNisCodeWasDefinedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.NisCode.Should().Be(municipalityNisCodeWasDefined.NisCode);
                    expectedVersion.PuriId.Should().Be($"{Namespace}/{municipalityNisCodeWasDefined.NisCode}");
                    expectedVersion.VersionTimestamp.Should().Be(municipalityNisCodeWasDefined.Provenance.Timestamp);
                    expectedVersion.CreatedOnTimestamp.Should().Be(municipalityWasRegistered.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityNisCodeWasCorrected()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityNisCodeWasCorrected = _fixture.Create<MunicipalityNisCodeWasCorrected>();
            var eventPosition = position + 1;
            var municipalityNisCodeWasCorrectedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNisCodeWasCorrected>(new Envelope(municipalityNisCodeWasCorrected, municipalityNisCodeWasCorrectedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.NisCode.Should().Be(municipalityNisCodeWasCorrected.NisCode);
                    expectedVersion.PuriId.Should().Be($"{Namespace}/{municipalityNisCodeWasCorrected.NisCode}");
                    expectedVersion.VersionTimestamp.Should().Be(municipalityNisCodeWasCorrected.Provenance.Timestamp);
                    expectedVersion.CreatedOnTimestamp.Should().Be(municipalityWasRegistered.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityWasNamed()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityWasNamed = _fixture.Create<MunicipalityWasNamed>();
            var eventPosition = position + 1;
            var municipalityWasNamedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasNamed>(new Envelope(municipalityWasNamed, municipalityWasNamedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityWasNamed.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.NameDutch.Should().Be(municipalityWasNamed.Name);
                            break;
                        case Language.French:
                            expectedVersion!.NameFrench.Should().Be(municipalityWasNamed.Name);
                            break;
                        case Language.German:
                            expectedVersion!.NameGerman.Should().Be(municipalityWasNamed.Name);
                            break;
                        case Language.English:
                            expectedVersion!.NameEnglish.Should().Be(municipalityWasNamed.Name);
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityWasNamed.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityNameWasCorrected()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityNameWasCorrected = _fixture.Create<MunicipalityNameWasCorrected>();
            var eventPosition = position + 1;
            var municipalityNameWasCorrectedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNameWasCorrected>(new Envelope(municipalityNameWasCorrected, municipalityNameWasCorrectedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityNameWasCorrected.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.NameDutch.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                        case Language.French:
                            expectedVersion!.NameFrench.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                        case Language.German:
                            expectedVersion!.NameGerman.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                        case Language.English:
                            expectedVersion!.NameEnglish.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityNameWasCorrected.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityNameWasCleared()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityNameWasCleared = _fixture.Create<MunicipalityNameWasCleared>();
            var eventPosition = position + 1;
            var municipalityNameWasClearedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNameWasCleared>(new Envelope(municipalityNameWasCleared, municipalityNameWasClearedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityNameWasCleared.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.NameDutch.Should().BeNull();
                            break;
                        case Language.French:
                            expectedVersion!.NameFrench.Should().BeNull();
                            break;
                        case Language.German:
                            expectedVersion!.NameGerman.Should().BeNull();
                            break;
                        case Language.English:
                            expectedVersion!.NameEnglish.Should().BeNull();
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityNameWasCleared.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityNameWasCorrectedToCleared()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityNameWasCorrectedToCleared = _fixture.Create<MunicipalityNameWasCorrectedToCleared>();
            var eventPosition = position + 1;
            var municipalityNameWasCorrectedToClearedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNameWasCorrectedToCleared>(new Envelope(municipalityNameWasCorrectedToCleared, municipalityNameWasCorrectedToClearedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityNameWasCorrectedToCleared.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.NameDutch.Should().BeNull();
                            break;
                        case Language.French:
                            expectedVersion!.NameFrench.Should().BeNull();
                            break;
                        case Language.German:
                            expectedVersion!.NameGerman.Should().BeNull();
                            break;
                        case Language.English:
                            expectedVersion!.NameEnglish.Should().BeNull();
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityNameWasCorrectedToCleared.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityOfficialLanguageWasAdded()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityOfficialLanguageWasAdded = _fixture.Create<MunicipalityOfficialLanguageWasAdded>();
            var eventPosition = position + 1;
            var municipalityOfficialLanguageWasAddedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityOfficialLanguageWasAdded>(new Envelope(municipalityOfficialLanguageWasAdded, municipalityOfficialLanguageWasAddedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityOfficialLanguageWasAdded.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.OfficialLanguageDutch.Should().BeTrue();
                            break;
                        case Language.French:
                            expectedVersion!.OfficialLanguageFrench.Should().BeTrue();
                            break;
                        case Language.German:
                            expectedVersion!.OfficialLanguageGerman.Should().BeTrue();
                            break;
                        case Language.English:
                            expectedVersion!.OfficialLanguageEnglish.Should().BeTrue();
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityOfficialLanguageWasAdded.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityOfficialLanguageWasRemoved()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityOfficialLanguageWasRemoved = _fixture.Create<MunicipalityOfficialLanguageWasRemoved>();
            var eventPosition = position + 1;
            var municipalityOfficialLanguageWasRemovedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityOfficialLanguageWasRemoved>(new Envelope(municipalityOfficialLanguageWasRemoved, municipalityOfficialLanguageWasRemovedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityOfficialLanguageWasRemoved.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.OfficialLanguageDutch.Should().BeFalse();
                            break;
                        case Language.French:
                            expectedVersion!.OfficialLanguageFrench.Should().BeFalse();
                            break;
                        case Language.German:
                            expectedVersion!.OfficialLanguageGerman.Should().BeFalse();
                            break;
                        case Language.English:
                            expectedVersion!.OfficialLanguageEnglish.Should().BeFalse();
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityOfficialLanguageWasRemoved.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityFacilityLanguageWasAdded()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityFacilityLanguageWasAdded = _fixture.Create<MunicipalityFacilityLanguageWasAdded>();
            var eventPosition = position + 1;
            var municipalityFacilityLanguageWasAddedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityFacilityLanguageWasAdded>(new Envelope(municipalityFacilityLanguageWasAdded, municipalityFacilityLanguageWasAddedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityFacilityLanguageWasAdded.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.FacilityLanguageDutch.Should().BeTrue();
                            break;
                        case Language.French:
                            expectedVersion!.FacilityLanguageFrench.Should().BeTrue();
                            break;
                        case Language.German:
                            expectedVersion!.FacilityLanguageGerman.Should().BeTrue();
                            break;
                        case Language.English:
                            expectedVersion!.FacilityLanguageEnglish.Should().BeTrue();
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityFacilityLanguageWasAdded.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityFacilityLanguageWasRemoved()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityFacilityLanguageWasRemoved = _fixture.Create<MunicipalityFacilityLanguageWasRemoved>();
            var eventPosition = position + 1;
            var municipalityFacilityLanguageWasRemovedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityFacilityLanguageWasRemoved>(new Envelope(municipalityFacilityLanguageWasRemoved, municipalityFacilityLanguageWasRemovedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    switch (municipalityFacilityLanguageWasRemoved.Language)
                    {
                        case Language.Dutch:
                            expectedVersion!.FacilityLanguageDutch.Should().BeFalse();
                            break;
                        case Language.French:
                            expectedVersion!.FacilityLanguageFrench.Should().BeFalse();
                            break;
                        case Language.German:
                            expectedVersion!.FacilityLanguageGerman.Should().BeFalse();
                            break;
                        case Language.English:
                            expectedVersion!.FacilityLanguageEnglish.Should().BeFalse();
                            break;
                    }
                    expectedVersion!.VersionTimestamp.Should().Be(municipalityFacilityLanguageWasRemoved.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityBecameCurrent()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityBecameCurrent = _fixture.Create<MunicipalityBecameCurrent>();
            var eventPosition = position + 1;
            var municipalityBecameCurrentMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityBecameCurrent>(new Envelope(municipalityBecameCurrent, municipalityBecameCurrentMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.Status.Should().Be(MunicipalityStatus.Current);
                    expectedVersion.OsloStatus.Should().Be("InGebruik");
                    expectedVersion.VersionTimestamp.Should().Be(municipalityBecameCurrent.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityWasCorrectedToCurrent()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityWasCorrectedToCurrent = _fixture.Create<MunicipalityWasCorrectedToCurrent>();
            var eventPosition = position + 1;
            var municipalityWasCorrectedToCurrentMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasCorrectedToCurrent>(new Envelope(municipalityWasCorrectedToCurrent, municipalityWasCorrectedToCurrentMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.Status.Should().Be(MunicipalityStatus.Current);
                    expectedVersion.OsloStatus.Should().Be("InGebruik");
                    expectedVersion.VersionTimestamp.Should().Be(municipalityWasCorrectedToCurrent.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityWasRetired()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityWasRetired = _fixture.Create<MunicipalityWasRetired>();
            var eventPosition = position + 1;
            var municipalityWasRetiredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasRetired>(new Envelope(municipalityWasRetired, municipalityWasRetiredMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.Status.Should().Be(MunicipalityStatus.Retired);
                    expectedVersion.OsloStatus.Should().Be("Gehistoreerd");
                    expectedVersion.VersionTimestamp.Should().Be(municipalityWasRetired.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityWasCorrectedToRetired()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityWasCorrectedToRetired = _fixture.Create<MunicipalityWasCorrectedToRetired>();
            var eventPosition = position + 1;
            var municipalityWasCorrectedToRetiredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasCorrectedToRetired>(new Envelope(municipalityWasCorrectedToRetired, municipalityWasCorrectedToRetiredMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.Status.Should().Be(MunicipalityStatus.Retired);
                    expectedVersion.OsloStatus.Should().Be("Gehistoreerd");
                    expectedVersion.VersionTimestamp.Should().Be(municipalityWasCorrectedToRetired.Provenance.Timestamp);
                });
        }

        [Fact]
        public async Task WhenMunicipalityWasRemoved()
        {
            var position = _fixture.Create<long>();
            var municipalityWasRegistered = _fixture.Create<MunicipalityWasRegistered>();
            var municipalityWasRegisteredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position }
            };
            var municipalityWasRemoved = _fixture.Create<MunicipalityWasRemoved>();
            var eventPosition = position + 1;
            var municipalityWasRemovedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, eventPosition }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasRemoved>(new Envelope(municipalityWasRemoved, municipalityWasRemovedMetadata)))
                .Then(async ct =>
                {
                    var expectedVersion = await ct.MunicipalityVersions.FindAsync(eventPosition);
                    expectedVersion.Should().NotBeNull();
                    expectedVersion!.IsRemoved.Should().BeTrue();
                    expectedVersion.VersionTimestamp.Should().Be(municipalityWasRemoved.Provenance.Timestamp);
                });
        }

        protected override MunicipalityVersionProjections CreateProjection()
            => new MunicipalityVersionProjections(
                new OptionsWrapper<IntegrationOptions>(new IntegrationOptions { Namespace = Namespace }));
    }
}
