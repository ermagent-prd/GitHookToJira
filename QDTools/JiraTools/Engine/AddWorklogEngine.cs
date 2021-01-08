using System;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraTools.Engine
{
    internal class AddWorklogEngine
    {
        #region Public methods

        public Worklog Execute(Issue issue, Worklog worklog, WorklogStrategy strategy)
        {
            var cmtTask = addWorkLog(issue, worklog, strategy);

            cmtTask.Wait();

            return cmtTask.Result;

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
