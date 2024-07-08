namespace MunicipalityRegistry.Tests.ImportApi.Merger
{
    using System.Threading;
    using Api.Import.Infrastructure.Vrbg;
    using Api.Import.Merger;
    using FluentAssertions;
    using FluentValidation;
    using Moq;
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
        public void ThenValidationErrorIsThrown()
        {
            var act = async () => await _controller.Propose(
                new ProposeMergersRequest(),
                new ProposeMergersRequestValidator(_legacyContext, _importContext),
                Mock.Of<IMunicipalityGeometryReader>(),
                CancellationToken.None);

            act
                .Should()
                .ThrowAsync<ValidationException>();
        }
    }
}
