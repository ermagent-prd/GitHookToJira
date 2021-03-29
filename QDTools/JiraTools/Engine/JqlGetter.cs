using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Parameters;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class JqlGetter
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

            options.StartAt = 0;

            options.MaxIssuesPerRequest = this.parameters.MaxIssuesPerRequest; //Max 100 ??

            var result = new List<Issue>();

            while (true)
            { 
                var task = GetIssues(options);

                task.Wait();

                if (!task.Result.Any())
                    break;

                options.StartAt += options.MaxIssuesPerRequest.Value;

                result.AddRange(task.Result);
            }

            return result;
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
