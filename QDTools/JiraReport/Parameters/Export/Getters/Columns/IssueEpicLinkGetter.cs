using System.Linq;
using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueEpicLinkGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            var epicLink = issue.CustomFields.FirstOrDefault(x => x.Name == "Epic Link");

            return epicLink != null ? epicLink.Name : "";
        }
    }
}
