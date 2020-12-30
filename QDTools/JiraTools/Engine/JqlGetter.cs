using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Parameters;
using JiraTools.Service;

namespace JiraTools.Engine
{
    internal class JqlGetter
    {
        #region Private properties

        private readonly ServiceManagerContainer requestFactory;

        private readonly IJiraToolsParameters parameters;

        #endregion

        #region Constructor

        public JqlGetter(
            ServiceManagerContainer requestFactory,
            IJiraToolsParameters parameters)
        {
            this.requestFactory = requestFactory;

            this.parameters = parameters;
        }

        #endregion

        #region Public methods

        public IEnumerable<Issue> Execute(string jsql)
        {
            var options = new IssueSearchOptions(jsql);

            options.MaxIssuesPerRequest = this.parameters.MaxIssuesPerRequest; //Max 100 ??

            var task = GetIssues(options);
            
            task.Wait();

            return task.Result;
        }


        #endregion

        #region Private methods

        private async Task<IPagedQueryResult<Issue>> GetIssues(IssueSearchOptions options)
        {

            return await this.requestFactory.Service.Issues.GetIssuesFromJqlAsync(options);
        }


        #endregion
    }
}
