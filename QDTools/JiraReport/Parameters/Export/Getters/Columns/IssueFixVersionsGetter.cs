using System;
using System.Linq;
using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueFixVersionsGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            var versions = issue.FixVersions.Select(x => x.Name).ToList();

            return versions != null && versions.Count > 0 ?
                String.Join(", ", versions) : "";
        }
    }
}
