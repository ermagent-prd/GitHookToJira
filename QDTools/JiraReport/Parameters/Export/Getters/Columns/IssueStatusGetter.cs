using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    class IssueStatusGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.Status.Name;
        }
    }
}
