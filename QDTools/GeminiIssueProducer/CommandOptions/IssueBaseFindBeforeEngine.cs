using Countersoft.Gemini.Api;
using GeminiIssueProducer.Commands;
using GeminiIssueProducer.Parameters;
using System.Linq;

namespace GeminiIssueProducer.CommandOptions
{
    internal abstract class IssueBaseFindBeforeEngine : IGeminiCommand
    {
        protected ServiceManager serviceManager;

        protected IssueBaseFindBeforeEngine(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public IGeminiCommandOutput Execute(IssueParams parameters)
        {
            var finder =
                new IssueFinderEngine(serviceManager);

            FindCommandOutput finderResult =
                finder.Execute(parameters);

            if (finderResult.Result)
            {
                if (finderResult.Value.Any())
                    return UpdateIssue(parameters, finderResult.Value);
                else
                    return AddIssue(parameters);
            }

            return finderResult;
        }

        protected abstract IGeminiCommandOutput AddIssue(IssueParams parameters);
        protected abstract IGeminiCommandOutput UpdateIssue(IssueParams parameters, int[] value);
    }
}
