using Atlassian.Jira;

namespace JiraTools.Engine
{
    public class LinkEngine
    {
        public void Execute(Issue issue, string parentKey, string linkName)
        {
            issue.LinkToIssueAsync(parentKey, linkName).Wait();
        }
    }
}
