using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueReporterGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.ReporterUser != null ? issue.ReporterUser.Username : "";
        }
    }
}
