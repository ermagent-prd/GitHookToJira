using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueAssigneeGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.AssigneeUser != null ? issue.AssigneeUser.DisplayName : "";
        }
    }
}
