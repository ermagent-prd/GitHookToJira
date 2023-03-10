using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueTypeGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.Type.Name;
        }
    }
}
