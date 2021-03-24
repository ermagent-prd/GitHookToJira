using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    class IssueDueDateGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.DueDate.ToString();
        }
    }
}
