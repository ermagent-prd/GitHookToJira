using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public IEnumerable<Issue> Execute(string projectCode, string issueCode)
        {
            var jira = requestFactory.Service;

            jira.Issues.MaxIssuesPerRequest = parameters.MaxIssuesPerRequest; //Max 100 ??

            // use LINQ syntax to retrieve issues
            return from i in jira.Issues.Queryable
                   where i.Key == issueCode   //i.Key to get a single specific item (TODOPL)
                   orderby i.Created
                   select i;

        }

        #endregion
    }
}
