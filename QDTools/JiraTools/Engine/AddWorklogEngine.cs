using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using JiraTools.Model;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class AddWorklogEngine
    {

        #region Private properties

        private readonly ServiceManagerContainer requestFactory;

        #endregion

        public AddWorklogEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        #region Public methods

        public Worklog Execute(Issue issue, Worklog worklog, WorklogStrategy strategy)
        {
            var cmtTask = addWorkLog(issue, worklog, strategy);

            cmtTask.Wait();

            return cmtTask.Result;

        }

        public void Execute(Issue issue, List<WorkLogInfo> workLogInfoList)
        {
            if (workLogInfoList == null)
                return;

            foreach(var workLogInfo in workLogInfoList)
                Execute(issue, workLogInfo.Author, workLogInfo.TimeSpent, workLogInfo.StartDate, workLogInfo.Comment);
        }


        public void Execute(Issue issue, string author, string timeSpent, DateTime startDate, string comment)
        {
            var worklog = new Worklog(timeSpent, startDate, comment)
            {
                Author = author
            };

            Execute(issue, worklog, WorklogStrategy.AutoAdjustRemainingEstimate);
        }

        public void Execute(string issueKey, string author, string timeSpent, DateTime startDate, string comment)
        {
            var worklog = new Worklog(timeSpent, startDate, comment)
            {
                Author = author
            };

            var task = addWorkLog(issueKey, worklog, WorklogStrategy.AutoAdjustRemainingEstimate);

            task.Wait();

        }

        #endregion

        #region Private methods

        private async Task<Worklog> addWorkLog(Issue issue, Worklog worklog, WorklogStrategy strategy)
        {

            return await issue.AddWorklogAsync(worklog, strategy);
        }

        private async Task<Worklog> addWorkLog(string issueKey, Worklog worklog, WorklogStrategy strategy)
        {
            var jira = this.requestFactory.Service;

            var issue = await jira.Issues.GetIssueAsync(issueKey);

            if (issue == null)
                return null;

            return await addWorkLog(issue, worklog, strategy);
        }


        #endregion
    }
}
