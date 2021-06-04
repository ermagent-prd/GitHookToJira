using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueRemainigEstimateGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.TimeTrackingData != null ? issue.TimeTrackingData.RemainingEstimate : "";
        }
    }
}
