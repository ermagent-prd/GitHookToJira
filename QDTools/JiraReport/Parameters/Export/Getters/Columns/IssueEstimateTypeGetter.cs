using System.Linq;
using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueEstimateTypeGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            var estimateType = issue.CustomFields.FirstOrDefault(x => x.Name == "Estimate Type");

            return estimateType != null ? estimateType.Name : "";
        }
    }
}
