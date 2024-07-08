namespace MunicipalityRegistry.Tests.ImportApi.Merger
{
    using System.Threading;
    using Api.Import.Merger;
    using FluentAssertions;
    using FluentValidation;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class WhenProposingMerger : ImportApiTest
    {
        private readonly MergerController _controller;
        private readonly FakeLegacyContext _legacyContext;
        private readonly FakeImportContext _importContext;

        public WhenProposingMerger(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _controller = CreateMergerControllerWithUser<MergerController>();

            _legacyContext = new FakeLegacyContextFactory().CreateDbContext();
            _importContext = new FakeImportContextFactory().CreateDbContext();
        }

        [Fact]
        public void ThenValidationErrorIsThrown()
        {
            var act = async () => await _controller.Propose(
                new ProposeMergersRequest(),
                new ProposeMergersRequestValidator(_legacyContext, _importContext),
                CancellationToken.None);

            act
                .Should()
                .ThrowAsync<ValidationException>();
        }
    }
}
