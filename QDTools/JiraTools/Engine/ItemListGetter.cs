using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using JiraTools.Parameters;
using JiraTools.Service;

namespace JiraTools.Engine
{
    internal class ItemListGetter
    {
        #region Private properties

        private readonly ServiceManagerContainer requestFactory;

        private readonly IJiraToolsParameters parameters;

        #endregion

        #region Public
        
        #endregion

        #region Constructor

        public ItemListGetter(
            ServiceManagerContainer requestFactory,
            IJiraToolsParameters parameters)
        {
            this.requestFactory = requestFactory;

            this.parameters = parameters;
        }

        #endregion

        #region Public methods

        public IEnumerable<Issue> Execute(string projectCode)
        {
            var jira = requestFactory.Service;

            jira.Issues.MaxIssuesPerRequest = parameters.MaxIssuesPerRequest; //Max 100 ??

            // use LINQ syntax to retrieve issues
            return from i in jira.Issues.Queryable
                   where i.Project == projectCode 
                   orderby i.Created
                   select i;

        }

        public IEnumerable<Issue> Execute(string filter, QuerableType type)
        {
            var jira = requestFactory.Service;

            jira.Issues.MaxIssuesPerRequest = parameters.MaxIssuesPerRequest; //Max 100 ??

            if (type == QuerableType.ByCode)
                // use LINQ syntax to retrieve issues
                return from i in jira.Issues.Queryable
                       where i.Key == filter
                       orderby i.Created
                       select i;
            else
                return from i in jira.Issues.Queryable
                       where i.Summary == filter
                       orderby i.Created
                       select i;



        }

        #endregion
    }
}
