using AlfrescoTools.Parameters;
using GeminiTools.Parameters;


namespace GeminiToJira.Parameters
{
    internal class AlfrescoParamContainer : IAlfrescoToolsParameters
    {
        public string ServerUrl => AlfrescoConstants.ServiceUrl;

        public string UserName => AlfrescoConstants.UserName;

        public string Password => AlfrescoConstants.Password;
    }
}
