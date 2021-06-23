using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class AddWatcherEngine
    {
        #region Private properties
        private readonly ServiceManagerContainer requestFactory;
        #endregion

        public AddWatcherEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }



        #region Public methods

        public void Execute(string issueKey, string stringAccountId)
        {
            addWatcher(issueKey, stringAccountId);

        }

        public void Execute(Issue issue, string accountId)
        {
            issue.AddWatcherAsync(accountId).Wait();
        }

        #endregion

        #region private method
        private async void addWatcher(string issueKey, string stringAccountId)
        {
           
            var jira = this.requestFactory.Service;

            var issue = await jira.Issues.GetIssueAsync(issueKey);

            if (issue == null)
                return;

            issue.AddWatcherAsync(stringAccountId).Wait();
        }

        #endregion

    }
}
