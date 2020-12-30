namespace JiraTools.Parameters
{
    public interface IJiraToolsParameters
    {
        string ServerUrl { get; }

        string User { get; }

        string Token { get; }

        string IssueApi { get; }
    }
}
