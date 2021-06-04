using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueAffectedVersionsGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            var affected = issue.AffectsVersions.Select(x => x.Name).ToList();

            return affected != null && affected.Count > 0 ?
                String.Join(", ", affected) : "";
        }
    }
}
