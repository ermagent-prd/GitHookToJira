using System.Linq;
using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueStartDateGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            var startDate = issue.CustomFields.FirstOrDefault(x => x.Name == "Start date");

            return startDate != null ? startDate.Values[0] : "";
        }
    }
}
