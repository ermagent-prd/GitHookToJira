using Atlassian.Jira;

namespace JiraTools.Engine
{
    public class LinkEngine
    {
        public void Execute(Issue issue, string parentKey)
        {
            issue.LinkToIssueAsync(parentKey, "Relates").Wait();
        }
    }
}
