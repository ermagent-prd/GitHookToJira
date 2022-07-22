using Atlassian.Jira;
using JiraTools.Service;
using System.Threading.Tasks;

namespace JiraTools.Engine
{
    public class IssueGetter
    {
        #region Private properties
        private readonly ServiceManagerContainer requestFactory;
        #endregion

        public IssueGetter(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }



        #region Public methods

        public Issue Execute(string issueKey)
        {
            var cmtTask = getIssue(issueKey);

            if (cmtTask == null)
                return null;

            cmtTask.Wait();

            return cmtTask.Result;
        }



        #endregion

        #region Private methods


        private async Task<Issue> getIssue(string issueKey)
        {
            try
            {
                var jira = this.requestFactory.Service;

                return await jira.Issues.GetIssueAsync(issueKey);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
