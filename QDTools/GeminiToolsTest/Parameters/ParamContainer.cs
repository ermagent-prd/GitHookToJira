using GeminiTools.Parameters;

namespace GeminiToolsTest.Parameters
{
    internal class ParamContainer : IGeminiToolsParameters
    {
        public string ServerUrl => Constants.GeminiUrl;
    }
}
