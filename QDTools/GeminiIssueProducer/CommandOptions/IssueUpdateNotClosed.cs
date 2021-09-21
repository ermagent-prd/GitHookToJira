using Countersoft.Gemini.Api;
using GeminiIssueProducer.Commands;
using GeminiIssueProducer.Parameters;
using System.Linq;

namespace GeminiIssueProducer.CommandOptions
{
    internal class IssueUpdateNotClosed : IssueBaseFindBeforeEngine
    {
        public IssueUpdateNotClosed(ServiceManager serviceManager) : base(serviceManager)
        {
        }

        protected override IGeminiCommandOutput UpdateIssue(IssueParams parameters, int[] foundIds)
        {
            var updater =
                new IssueUpdateCommentEngine(serviceManager);

            foreach (var foundId in foundIds)
            {
                var updaterResult =
                    updater.Execute(parameters, foundId);

                if (!updaterResult.Result)
                    return updaterResult;
            }

            return new SimpleCommandOutput(true, foundIds.Last()); //return just one of the IDs found
        }

        protected override IGeminiCommandOutput AddIssue(IssueParams parameters)
        {
            // no action
            return new SimpleCommandOutput(true, GeminiConstants.OK);
        }
    }
}
