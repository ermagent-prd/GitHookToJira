using Countersoft.Gemini.Commons.Dto;
using JiraTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Engine
{
    public class TimeLogEngine
    {
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;

        public TimeLogEngine(
            JiraAccountIdEngine accountEngine,
            ParseCommentEngine parseCommentEngine)
        {
            this.accountEngine = accountEngine;
            this.parseCommentEngine = parseCommentEngine;
        }

        public List<WorkLogInfo> Execute(List<IssueTimeTrackingDto> timeEntries)
        {
            List<WorkLogInfo> result = new List<WorkLogInfo>();

            foreach (var timeEntry in timeEntries)
            {
                var author = accountEngine.Execute(timeEntry.Fullname);
                var comment = "[~accountId:" + author.AccountId + "]\n" + parseCommentEngine.Execute(timeEntry.Entity.Comment);
                var timeSpent = timeEntry.Entity.Hours + "h " + timeEntry.Entity.Minutes + "m";

                result.Add(new WorkLogInfo(author.AccountId, timeEntry.Entity.EntryDate, timeSpent, comment));
            }

            return result;
        }

    }
}
