using AlfrescoTools.Parameters;
using GeminiTools.Parameters;


namespace GeminiToJira.Parameters
{
    internal class AlfrescoParamContainer : IAlfrescoToolsParameters
    {
        public string ServerUrl => AlfrescoConstants.ServiceUrl;

        public string ALFRESCO_PATH => AlfrescoConstants.Alfresco_PATH;

        public string SAVING_PATH => AlfrescoConstants.SAVING_PATH;

        public string UserName => AlfrescoConstants.UserName;

        public string Password => AlfrescoConstants.Password;

        public string AttachmentPath => AlfrescoConstants.AttachmentPath;
    }
}
