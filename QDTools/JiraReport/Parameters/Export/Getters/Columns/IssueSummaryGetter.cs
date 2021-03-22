using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    class IssueSummaryGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.Summary;
        }
    }
}
