namespace SvnTools.Parameters
{
    public interface ISvnToolsParameters
    {
        string ServerUrl { get; }

        string TrackingIssuePattern { get; }

        bool WindowsAuth { get; }

        string User { get; }

        string Password { get; }
    }
}
