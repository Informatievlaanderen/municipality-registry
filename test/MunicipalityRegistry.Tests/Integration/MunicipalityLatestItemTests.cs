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

    public class MunicipalityLatestItemTests : IntegrationProjectionTest<MunicipalityLatestItemProjections>
    {
        private const string Namespace = "https://data.vlaanderen.be/id/gemeente";
        private readonly Fixture _fixture;

        public MunicipalityLatestItemTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
            _fixture.Customize(new WithFixedMunicipalityId());
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
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityWasRegistered.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    expectedLatestItem!.NisCode.Should().Be(municipalityWasRegistered.NisCode);
                    expectedLatestItem.Namespace.Should().Be(Namespace);
                    expectedLatestItem.PuriId.Should().Be($"{Namespace}/{municipalityWasRegistered.NisCode}");
                    expectedLatestItem.IdempotenceKey.Should().Be(position);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityWasRegistered.Provenance.Timestamp);
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
            var municipalityNisCodeWasDefinedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNisCodeWasDefined>(new Envelope(municipalityNisCodeWasDefined, municipalityNisCodeWasDefinedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityNisCodeWasDefined.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    expectedLatestItem!.NisCode.Should().Be(municipalityNisCodeWasDefined.NisCode);
                    expectedLatestItem.PuriId.Should().Be($"{Namespace}/{municipalityNisCodeWasDefined.NisCode}");
                    expectedLatestItem.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityNisCodeWasDefined.Provenance.Timestamp);
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
            var municipalityNisCodeWasCorrectedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNisCodeWasCorrected>(new Envelope(municipalityNisCodeWasCorrected, municipalityNisCodeWasCorrectedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityNisCodeWasCorrected.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    expectedLatestItem!.NisCode.Should().Be(municipalityNisCodeWasCorrected.NisCode);
                    expectedLatestItem.PuriId.Should().Be($"{Namespace}/{municipalityNisCodeWasCorrected.NisCode}");
                    expectedLatestItem.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityNisCodeWasCorrected.Provenance.Timestamp);
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
            var municipalityWasNamedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasNamed>(new Envelope(municipalityWasNamed, municipalityWasNamedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityWasNamed.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityWasNamed.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.NameDutch.Should().Be(municipalityWasNamed.Name);
                            break;
                        case Language.French:
                            expectedLatestItem!.NameFrench.Should().Be(municipalityWasNamed.Name);
                            break;
                        case Language.German:
                            expectedLatestItem!.NameGerman.Should().Be(municipalityWasNamed.Name);
                            break;
                        case Language.English:
                            expectedLatestItem!.NameEnglish.Should().Be(municipalityWasNamed.Name);
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityWasNamed.Provenance.Timestamp);
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
            var municipalityNameWasCorrectedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNameWasCorrected>(new Envelope(municipalityNameWasCorrected, municipalityNameWasCorrectedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityNameWasCorrected.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityNameWasCorrected.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.NameDutch.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                        case Language.French:
                            expectedLatestItem!.NameFrench.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                        case Language.German:
                            expectedLatestItem!.NameGerman.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                        case Language.English:
                            expectedLatestItem!.NameEnglish.Should().Be(municipalityNameWasCorrected.Name);
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityNameWasCorrected.Provenance.Timestamp);
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
            var municipalityNameWasClearedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNameWasCleared>(new Envelope(municipalityNameWasCleared, municipalityNameWasClearedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityNameWasCleared.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityNameWasCleared.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.NameDutch.Should().BeNull();
                            break;
                        case Language.French:
                            expectedLatestItem!.NameFrench.Should().BeNull();
                            break;
                        case Language.German:
                            expectedLatestItem!.NameGerman.Should().BeNull();
                            break;
                        case Language.English:
                            expectedLatestItem!.NameEnglish.Should().BeNull();
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityNameWasCleared.Provenance.Timestamp);
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
            var municipalityNameWasCorrectedToClearedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityNameWasCorrectedToCleared>(new Envelope(municipalityNameWasCorrectedToCleared, municipalityNameWasCorrectedToClearedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityNameWasCorrectedToCleared.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityNameWasCorrectedToCleared.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.NameDutch.Should().BeNull();
                            break;
                        case Language.French:
                            expectedLatestItem!.NameFrench.Should().BeNull();
                            break;
                        case Language.German:
                            expectedLatestItem!.NameGerman.Should().BeNull();
                            break;
                        case Language.English:
                            expectedLatestItem!.NameEnglish.Should().BeNull();
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityNameWasCorrectedToCleared.Provenance.Timestamp);
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
            var municipalityOfficialLanguageWasAddedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityOfficialLanguageWasAdded>(new Envelope(municipalityOfficialLanguageWasAdded, municipalityOfficialLanguageWasAddedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityOfficialLanguageWasAdded.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityOfficialLanguageWasAdded.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.OfficialLanguageDutch.Should().BeTrue();
                            break;
                        case Language.French:
                            expectedLatestItem!.OfficialLanguageFrench.Should().BeTrue();
                            break;
                        case Language.German:
                            expectedLatestItem!.OfficialLanguageGerman.Should().BeTrue();
                            break;
                        case Language.English:
                            expectedLatestItem!.OfficialLanguageEnglish.Should().BeTrue();
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityOfficialLanguageWasAdded.Provenance.Timestamp);
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
            var municipalityOfficialLanguageWasRemovedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityOfficialLanguageWasRemoved>(new Envelope(municipalityOfficialLanguageWasRemoved, municipalityOfficialLanguageWasRemovedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityOfficialLanguageWasRemoved.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityOfficialLanguageWasRemoved.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.OfficialLanguageDutch.Should().BeFalse();
                            break;
                        case Language.French:
                            expectedLatestItem!.OfficialLanguageFrench.Should().BeFalse();
                            break;
                        case Language.German:
                            expectedLatestItem!.OfficialLanguageGerman.Should().BeFalse();
                            break;
                        case Language.English:
                            expectedLatestItem!.OfficialLanguageEnglish.Should().BeFalse();
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityOfficialLanguageWasRemoved.Provenance.Timestamp);
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
            var municipalityFacilityLanguageWasAddedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityFacilityLanguageWasAdded>(new Envelope(municipalityFacilityLanguageWasAdded, municipalityFacilityLanguageWasAddedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityFacilityLanguageWasAdded.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityFacilityLanguageWasAdded.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.FacilityLanguageDutch.Should().BeTrue();
                            break;
                        case Language.French:
                            expectedLatestItem!.FacilityLanguageFrench.Should().BeTrue();
                            break;
                        case Language.German:
                            expectedLatestItem!.FacilityLanguageGerman.Should().BeTrue();
                            break;
                        case Language.English:
                            expectedLatestItem!.FacilityLanguageEnglish.Should().BeTrue();
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityFacilityLanguageWasAdded.Provenance.Timestamp);
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
            var municipalityFacilityLanguageWasRemovedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityFacilityLanguageWasRemoved>(new Envelope(municipalityFacilityLanguageWasRemoved, municipalityFacilityLanguageWasRemovedMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityFacilityLanguageWasRemoved.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    switch (municipalityFacilityLanguageWasRemoved.Language)
                    {
                        case Language.Dutch:
                            expectedLatestItem!.FacilityLanguageDutch.Should().BeFalse();
                            break;
                        case Language.French:
                            expectedLatestItem!.FacilityLanguageFrench.Should().BeFalse();
                            break;
                        case Language.German:
                            expectedLatestItem!.FacilityLanguageGerman.Should().BeFalse();
                            break;
                        case Language.English:
                            expectedLatestItem!.FacilityLanguageEnglish.Should().BeFalse();
                            break;
                    }
                    expectedLatestItem!.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityFacilityLanguageWasRemoved.Provenance.Timestamp);
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
            var municipalityBecameCurrentMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityBecameCurrent>(new Envelope(municipalityBecameCurrent, municipalityBecameCurrentMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityBecameCurrent.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    expectedLatestItem!.Status.Should().Be("InGebruik");
                    expectedLatestItem.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityBecameCurrent.Provenance.Timestamp);
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
            var municipalityWasCorrectedToCurrentMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasCorrectedToCurrent>(new Envelope(municipalityWasCorrectedToCurrent, municipalityWasCorrectedToCurrentMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityWasCorrectedToCurrent.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    expectedLatestItem!.Status.Should().Be("InGebruik");
                    expectedLatestItem.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityWasCorrectedToCurrent.Provenance.Timestamp);
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
            var municipalityWasRetiredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasRetired>(new Envelope(municipalityWasRetired, municipalityWasRetiredMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityWasRetired.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    expectedLatestItem!.Status.Should().Be("Gehistoreerd");
                    expectedLatestItem.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityWasRetired.Provenance.Timestamp);
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
            var municipalityWasCorrectedToRetiredMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, _fixture.Create<string>() },
                { Envelope.PositionMetadataKey, position + 1 }
            };

            await Sut
                .Given(new Envelope<MunicipalityWasRegistered>(new Envelope(municipalityWasRegistered, municipalityWasRegisteredMetadata)),
                    new Envelope<MunicipalityWasCorrectedToRetired>(new Envelope(municipalityWasCorrectedToRetired, municipalityWasCorrectedToRetiredMetadata)))
                .Then(async ct =>
                {
                    var expectedLatestItem =
                        await ct.MunicipalityLatestItems.FindAsync(municipalityWasCorrectedToRetired.MunicipalityId);
                    expectedLatestItem.Should().NotBeNull();
                    expectedLatestItem!.Status.Should().Be("Gehistoreerd");
                    expectedLatestItem.IdempotenceKey.Should().Be(position + 1);
                    expectedLatestItem.VersionTimestamp.Should().Be(municipalityWasCorrectedToRetired.Provenance.Timestamp);
                });
        }

        protected override MunicipalityLatestItemProjections CreateProjection()
            => new MunicipalityLatestItemProjections(
                new OptionsWrapper<IntegrationOptions>(new IntegrationOptions { Namespace = Namespace }));
    }
}
