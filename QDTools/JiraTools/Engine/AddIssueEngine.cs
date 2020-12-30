using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Service;

namespace JiraTools.Engine
{
    internal class AddIssueEngine
    {
        private readonly ServiceManagerContainer requestFactory;

        public AddIssueEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        public Issue Execute()
        {
            var task = addIssue();

            task.Wait();

            return task.Result;
        }

        #region Private methods

        private async Task<Issue> addIssue()
        {
            var newIssue = requestFactory.Service.CreateIssue("ER");

            newIssue.Type = "Bug";
            newIssue.Priority = "Medium";
            newIssue.Summary = "Issue Summary";
            newIssue.Description = "Test via api con Atlassian SDK";

            return await newIssue.SaveChangesAsync();
        }

        #endregion

    }
}
