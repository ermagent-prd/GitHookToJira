using GeminiTools.Parameters;


namespace GeminiToJira.Parameters
{
    internal class GeminiParamContainer : IGeminiToolsParameters
    {
        public string ServerUrl => GeminiConstants.GeminiUrl;

    }
}
