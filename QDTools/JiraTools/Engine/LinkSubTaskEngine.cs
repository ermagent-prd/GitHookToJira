using Atlassian.Jira;

namespace JiraTools.Engine
{
    public class LinkSubTaskEngine
    {
        public void Execute(Issue issue, string parentKey)
        {
            issue.LinkToIssueAsync(parentKey, "Subtask").Wait();
        }
    }
}
