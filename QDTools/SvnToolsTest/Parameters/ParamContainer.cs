

using SvnTools.Parameters;

namespace SvnToolsTest.Parameters
{
    public class ParamContainer : ISvnToolsParameters
    {
        public string ServerUrl => SvnTestConstants.SvnUrl;

        public string TrackingIssuePattern => SvnTestConstants.TrackingIssuePattern;

        public bool WindowsAuth => SvnTestConstants.WindowsAuth;

        public string User => SvnTestConstants.User;

        public string Password => SvnTestConstants.Password;
    }
}
