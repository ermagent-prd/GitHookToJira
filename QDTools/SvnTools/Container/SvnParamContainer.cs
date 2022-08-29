

using SvnTools.Parameters;

namespace SvnTools.Container
{
    public class SvnParamContainer : ISvnToolsParameters
    {
        private SvnToolsParameters param;

        public SvnParamContainer(SvnToolsParameters param)
        {
            this.param = param;
        }

        public string ServerUrl => param.ServerUrl;

        public string TrackingIssuePattern => param.TrackingIssuePattern;

        public bool WindowsAuth => param.WindowsAuth;

        public string User => param.User;

        public string Password => param.Password;

    }
}
