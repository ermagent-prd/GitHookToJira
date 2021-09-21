using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using GeminiIssueProducer.Helpers;
using GeminiIssueProducer.Parameters;

namespace GeminiIssueProducer.Commands
{
    internal class IssueUpdateCommentEngine
    {
        private readonly UpdateCommentHelper updateCommentHelper;

        public IssueUpdateCommentEngine(ServiceManager serviceManager)
        {
            this.updateCommentHelper =
                new UpdateCommentHelper(serviceManager);
        }

        public IGeminiCommandOutput Execute(IssueParams parameters, int issueId)
        {
            if (string.IsNullOrWhiteSpace(parameters.FreeParams.Comment))
                return new SimpleCommandOutput(false, GeminiConstants.ERR_EMPTY_COMMENT);

            int errCode =
                InternalExecute(issueId, parameters);

            return new SimpleCommandOutput(
               errCode == GeminiConstants.OK,
               errCode);
        }

        private int InternalExecute(int id, IssueParams parameters)
        {
            IssueCommentDto commentObj =
                updateCommentHelper.Execute(id, parameters);

            if (commentObj != null)
                return GeminiConstants.OK;

            return
                GeminiConstants.ERR_CANNOT_UPDATE_COMMENT;
        }
    }
}
