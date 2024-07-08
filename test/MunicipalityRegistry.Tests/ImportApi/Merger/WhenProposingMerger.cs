namespace MunicipalityRegistry.Tests.ImportApi.Merger
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Import.Infrastructure.Vrbg;
    using Api.Import.Merger;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using FluentValidation;
    using Moq;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO;
    using Projections.Legacy.MunicipalityDetail;
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
        public async Task GivenValidRequest_ThenMunicipalityMergersAreProposed()
        {
            LegacyContext.MunicipalityDetail.Add(new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10001"
            });
            LegacyContext.MunicipalityDetail.Add(new MunicipalityDetail
            {
                MunicipalityId = Guid.NewGuid(),
                NisCode = "10002"
            });
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
                .Setup(x => x.GetGeometry(It.IsAny<string>()))
                .Returns(async () => new WKTReader().Read("POLYGON((0 0,0 1,1 1,1 0,0 0))"));

            await _controller.Propose(
                request,
                new ProposeMergersRequestValidator(LegacyContext, ImportContext),
                municipalityGeometryReaderMock.Object,
                CancellationToken.None);

            //TODO-rik finish test
            //- worden commands gedispatched voor onbestaande gemeenten
            var streamStore = Container.Resolve<IStreamStore>();
            //var messages = await streamStore.ReadAllForwards(0, 5);

            //- test of de merge muni geometries wordt opgeroepen
            //zie municipalityGeometryReaderMock

            //- check of de ImportContext.MunicipalityMergers correct wordt opgevuld
        }
    }
}
