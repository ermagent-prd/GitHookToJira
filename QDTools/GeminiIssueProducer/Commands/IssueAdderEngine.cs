using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using GeminiIssueProducer.Parameters;

namespace GeminiIssueProducer.Commands
{
    internal class IssueAdderEngine
    {
        private readonly IssueCreateEngine createEngine;

        private readonly IssueUpdateCustomFieldEngine updateCustomFieldEngine;

        private readonly IssueUpdateAttachmentsEngine updateAttachmentsEngine;


        public IssueAdderEngine(ServiceManager serviceManager)
        {
            this.createEngine = 
                new IssueCreateEngine(serviceManager);

            this.updateCustomFieldEngine =
                new IssueUpdateCustomFieldEngine(serviceManager);

            this.updateAttachmentsEngine =
                new IssueUpdateAttachmentsEngine(serviceManager);
        }

        public IGeminiCommandOutput Execute(IssueParams parameters)
        {
            int errCode =
                InternalExecute(parameters);

            return new SimpleCommandOutput(
                errCode >= GeminiConstants.OK,
                errCode);
        }

        private int InternalExecute(IssueParams parameters)
        {
            CreateCommandOutput createCommandOutput =
                createEngine.Execute(parameters);

            if (!createCommandOutput.Result)
                return createCommandOutput.ErrorCode;

            IssueDto newIssue =
                createCommandOutput.Value;

            IGeminiCommandOutput customFieldsOutput =
                updateCustomFieldEngine.Execute(
                    newIssue, 
                    parameters);

            if (!customFieldsOutput.Result)
                return customFieldsOutput.ErrorCode;

            IGeminiCommandOutput attachmentOutput =
                updateAttachmentsEngine.Execute(
                    parameters,
                    newIssue.Id,
                    false);

            if (!attachmentOutput.Result)
                return attachmentOutput.ErrorCode;

            return newIssue.Id;
        }
    }
}
