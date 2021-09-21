using Countersoft.Gemini.Api;
using GeminiIssueProducer.Commands;
using GeminiIssueProducer.Parameters;
using System.IO;
using System.Linq;

namespace GeminiIssueProducer.CommandOptions
{
    internal class IssueAddOrUpdate : IssueBaseFindBeforeEngine
    {
        public IssueAddOrUpdate(ServiceManager serviceManager) : base(serviceManager)
        {
        }

        protected override IGeminiCommandOutput UpdateIssue(IssueParams parameters, int[] foundIds)
        {
            if (!TestParams(parameters))
                return new SimpleCommandOutput(false, GeminiConstants.ERR_FILE_NOT_EXISTS);

            var updater =
                new IssueUpdateEngine(serviceManager);

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
            if (!TestParams(parameters))
                return new SimpleCommandOutput(false, GeminiConstants.ERR_FILE_NOT_EXISTS);

            var adder =
                new IssueAdderEngine(serviceManager);

            return
                adder.Execute(parameters);
        }

        private bool TestParams(IssueParams issueParams)
        {
            return
                issueParams.FreeParams.Attachments.All(f => File.Exists(f));
        }
    }
}
