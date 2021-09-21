using Countersoft.Gemini.Api;
using GeminiIssueProducer.Parameters;

namespace GeminiIssueProducer.Commands
{
    internal class IssueUpdateEngine
    {
        private readonly IssueUpdateCommentEngine updateCommentEngine;

        private readonly IssueUpdateAttachmentsEngine attachmentsEngine;

        public IssueUpdateEngine(ServiceManager serviceManager)
        {
            this.updateCommentEngine =
                new IssueUpdateCommentEngine(serviceManager);

            this.attachmentsEngine =
                new IssueUpdateAttachmentsEngine(serviceManager);
        }

        public IGeminiCommandOutput Execute(IssueParams parameters, int issueId)
        {
            IGeminiCommandOutput updateOutput =
                updateCommentEngine.Execute(parameters, issueId);

            if(updateOutput.Result)
                updateOutput =
                    attachmentsEngine.Execute(parameters, issueId, true);
            
            return updateOutput;
        }

    }
}
