using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using GeminiIssueProducer.Helpers;
using GeminiIssueProducer.Parameters;
using System.Collections.Generic;

namespace GeminiIssueProducer.Commands
{
    internal class IssueCreateEngine
    {
        private readonly UserHelper userHelper;
        private readonly IssueHelper issueHelper;


        public IssueCreateEngine(ServiceManager serviceManager)
        {
            this.userHelper = 
                new UserHelper(serviceManager);

            this.issueHelper =
                new IssueHelper(serviceManager);
        }

        public CreateCommandOutput Execute(IssueParams parameters)
        {
            List<int> resourceIds =
                GetResourceIDs(parameters.FreeParams.ResourceNames);

            if (resourceIds == null)
                return new CreateCommandOutput(false, GeminiConstants.ERR_NO_RES_USER, null);

            IssueDto newIssue =
                CreateNewIssue(parameters, resourceIds);

            if (newIssue == null)
                return new CreateCommandOutput(false, GeminiConstants.ERR_CANNOT_CREATE_ISSUE, null);

            RemoveFollwer(newIssue.Id, parameters.FixedParams.UserId);

            return new CreateCommandOutput(true, GeminiConstants.OK, newIssue);
        }

        private List<int> GetResourceIDs(string[] resourceNames)
        {
            return
                userHelper.GetIds(resourceNames);
        }

        private IssueDto CreateNewIssue(IssueParams parameters, List<int> resourceIds)
        {
            return
                issueHelper.Execute(
                    parameters,
                    resourceIds);
        }
        private void RemoveFollwer(int issueId, int userId)
        {
            userHelper.RemoveFollower(
                issueId, 
                userId);
        }
    }
}
