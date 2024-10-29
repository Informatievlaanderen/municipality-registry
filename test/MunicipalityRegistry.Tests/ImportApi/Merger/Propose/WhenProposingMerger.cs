namespace MunicipalityRegistry.Tests.ImportApi.Merger.Propose
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Exceptions;
    using FluentAssertions;
    using FluentValidation;
    using Moq;
    using MunicipalityRegistry.Api.Import.Infrastructure.Vrbg;
    using MunicipalityRegistry.Api.Import.Merger;
    using MunicipalityRegistry.Api.Import.Merger.Propose;
    using MunicipalityRegistry.Projections.Legacy.MunicipalityDetail;
    using NetTopologySuite.IO;
    using SqlStreamStore;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class WhenProposingMerger : ImportApiTest
    {
        private readonly MergerController _controller;

        public WhenProposingMerger(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _controller = CreateMergerControllerWithUser<MergerController>();
        }

        [Fact]
        public void GivenInvalidRequest_ThenValidationErrorIsThrown()
        {
            var act = async () => await _controller.Propose(
                new ProposeMergersRequest(),
                new ProposeMergersRequestValidator(LegacyContext, ImportContext),
                Mock.Of<IMunicipalityGeometryReader>(),
                CancellationToken.None);

            act
                .Should()
                .ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task GivenValidRequestWithNewDestinationMunicipality_ThenMunicipalityMergersAreProposed()
        {
            var municipality1 = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10001"
            };
            var municipality2 = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10002"
            };

            LegacyContext.MunicipalityDetail.Add(municipality1);
            LegacyContext.MunicipalityDetail.Add(municipality2);
            await LegacyContext.SaveChangesAsync();

            var request = new ProposeMergersRequest
            {
                MergerYear = 2025,
                Municipalities =
                [
                    new ProposeMergerRequest
                    {
                        NisCode = "10000",
                        MergerOf = ["10001", "10002"],
                        ProposeMunicipality = new ProposeMergerMunicipalityRequest
                        {
                            OfficialLanguages = [Taal.NL],
                            Names = new Dictionary<Taal, string>
                            {
                                { Taal.NL, "Fusie NL"}
                            }
                        }
                    }
                ]
            };

            var municipalityGeometryReaderMock = new Mock<IMunicipalityGeometryReader>();
            municipalityGeometryReaderMock
                .Setup(x => x.GetGeometry("10000"))
                .Throws(() => new InvalidPolygonException());
            municipalityGeometryReaderMock
                .Setup(x => x.GetGeometry("10001"))
                .Returns(() => Task.FromResult(new WKTReader().Read("SRID=31370;POLYGON((0 0,0 1,1 1,1 0,0 0))")));
            municipalityGeometryReaderMock
                .Setup(x => x.GetGeometry("10002"))
                .Returns(() => Task.FromResult(new WKTReader().Read("SRID=31370;POLYGON((10 0,10 1,11 1,11 0,10 0))")));

            await _controller.Propose(
                request,
                new ProposeMergersRequestValidator(LegacyContext, ImportContext),
                municipalityGeometryReaderMock.Object,
                CancellationToken.None);

            var streamStore = Container.Resolve<IStreamStore>();
            var messages = await streamStore.ReadAllForwards(0, 4);
            messages.Messages.Length.Should().Be(4);
            var streamIds = messages.Messages.Select(x => x.StreamId).Distinct().ToList();
            streamIds.Count.Should().Be(1);
            var newMunicipalityId = streamIds.Single();

            municipalityGeometryReaderMock.Invocations.Count.Should().Be(3);

            ImportContext.MunicipalityMergers.Count().Should().Be(2);
            var merge1 = ImportContext.MunicipalityMergers.ToList()[0];
            merge1.MunicipalityId.Should().Be(municipality1.MunicipalityId.Value);
            merge1.Year.Should().Be(request.MergerYear);

            var merge2 = ImportContext.MunicipalityMergers.ToList()[1];
            merge2.MunicipalityId.Should().Be(municipality2.MunicipalityId.Value);
            merge2.Year.Should().Be(request.MergerYear);

            merge1.NewMunicipalityId.Should().Be(merge2.NewMunicipalityId);
            merge1.NewMunicipalityId.Should().Be(Guid.Parse(newMunicipalityId));
        }

        [Fact]
        public async Task GivenValidRequestWithExistingDestinationMunicipality_ThenMunicipalityMergersAreProposed()
        {
            var municipality1 = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10001"
            };
            var municipality2 = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10002"
            };
            var mergeMunicipality = new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10000"
            };

            LegacyContext.MunicipalityDetail.Add(municipality1);
            LegacyContext.MunicipalityDetail.Add(municipality2);
            LegacyContext.MunicipalityDetail.Add(mergeMunicipality);
            await LegacyContext.SaveChangesAsync();

            var request = new ProposeMergersRequest
            {
                MergerYear = 2025,
                Municipalities =
                [
                    new ProposeMergerRequest
                    {
                        NisCode = "10000",
                        MergerOf = ["10001", "10002"],
                        ProposeMunicipality = new ProposeMergerMunicipalityRequest
                        {
                            OfficialLanguages = [Taal.NL],
                            Names = new Dictionary<Taal, string>
                            {
                                { Taal.NL, "Fusie NL"}
                            }
                        }
                    }
                ]
            };

            var municipalityGeometryReaderMock = new Mock<IMunicipalityGeometryReader>();

            await _controller.Propose(
                request,
                new ProposeMergersRequestValidator(LegacyContext, ImportContext),
                municipalityGeometryReaderMock.Object,
                CancellationToken.None);

            var streamStore = Container.Resolve<IStreamStore>();
            var messages = await streamStore.ReadAllForwards(0, 1);
            messages.Messages.Length.Should().Be(0);

            var newMunicipalityId = mergeMunicipality.MunicipalityId.Value;

            municipalityGeometryReaderMock.Invocations.Count.Should().Be(0);

            ImportContext.MunicipalityMergers.Count().Should().Be(2);
            var merge1 = ImportContext.MunicipalityMergers.ToList()[0];
            merge1.MunicipalityId.Should().Be(municipality1.MunicipalityId.Value);
            merge1.Year.Should().Be(request.MergerYear);

            var merge2 = ImportContext.MunicipalityMergers.ToList()[1];
            merge2.MunicipalityId.Should().Be(municipality2.MunicipalityId.Value);
            merge2.Year.Should().Be(request.MergerYear);

            merge1.NewMunicipalityId.Should().Be(merge2.NewMunicipalityId);
            merge1.NewMunicipalityId.Should().Be(newMunicipalityId);
        }
    }
}
