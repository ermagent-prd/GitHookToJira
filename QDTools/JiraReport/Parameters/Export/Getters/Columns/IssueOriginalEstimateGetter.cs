using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    class IssueOriginalEstimateGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            return issue.TimeTrackingData != null ? issue.TimeTrackingData.OriginalEstimate : "";
        }
    }
}
