using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueCreatedGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.Created.HasValue ? issue.Created.Value.ToString("yyyy/MM/dd") : "";
        }
    }
}
