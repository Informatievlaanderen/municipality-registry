namespace MunicipalityRegistry.Api.Oslo.Municipality
{
    using System;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public sealed class ChangeFeedResult : ContentResult
    {
        public const string PageCompleteHeaderName = "X-Page-Complete";

        private readonly bool _isComplete;

        public ChangeFeedResult(string content, bool isComplete)
        {
            StatusCode = StatusCodes.Status200OK;
            ContentType = AcceptTypes.JsonCloudEventsBatch;
            Content = content;

            _isComplete = isComplete;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            AddIsCompleteHeader(context);
            await base.ExecuteResultAsync(context);
        }

        public override void ExecuteResult(ActionContext context)
        {
            AddIsCompleteHeader(context);
            base.ExecuteResult(context);
        }

        private void AddIsCompleteHeader(ActionContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.HttpContext.Response.Headers.Append(PageCompleteHeaderName, _isComplete.ToString());
        }
    }
}
