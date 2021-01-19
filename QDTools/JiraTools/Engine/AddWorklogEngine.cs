using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Model;

namespace JiraTools.Engine
{
    public class AddWorklogEngine
    {
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
            var worklog = new Worklog(timeSpent,startDate, comment);

            worklog.Author = author;

            Execute(issue, worklog, WorklogStrategy.AutoAdjustRemainingEstimate);
        }


        #endregion

        #region Private methods

        private async Task<Worklog> addWorkLog(Issue issue, Worklog worklog, WorklogStrategy strategy)
        {

            return await issue.AddWorklogAsync(worklog, strategy);
        }

        #endregion
    }
}
