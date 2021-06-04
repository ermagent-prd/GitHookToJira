using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters
{
    internal interface IExcelFieldGetter
    {
        string Execute(Issue issue);
    }
}
