using GeminiTools.Parameters;

namespace GeminiToolsTest.Parameters
{
    internal class ParamContainer : IGeminiToolsParameters
    {
        public string ServerUrl => GeminiToolsTestConstants.GeminiUrl;

        public string GEMINI_PATH => GeminiToolsTestConstants.GEMINI_PATH;

        public string SAVING_PATH => GeminiToolsTestConstants.SAVING_PATH;


    }
}
