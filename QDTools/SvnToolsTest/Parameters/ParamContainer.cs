

using SvnTools.Parameters;

namespace SvnToolsTest.Parameters
{
    public class ParamContainer : ISvnToolsParameters
    {
        public string ServerUrl => SvnTestConstants.SvnUrl;
    }
}
