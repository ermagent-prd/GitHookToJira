using GeminiTools.Parameters;


namespace GeminiToJira.Parameters
{
    internal class GeminiParamContainer : IGeminiToolsParameters
    {
        public string ServerUrl => GeminiConstants.GeminiUrl;

        public string GEMINI_PATH => GeminiConstants.GEMINI_PATH;

        public string SAVING_PATH => GeminiConstants.SAVING_PATH;

    }
}
