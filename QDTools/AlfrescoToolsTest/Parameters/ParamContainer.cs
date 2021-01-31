
using AlfrescoTools.Parameters;

namespace AlfrescoToolsTest.Parameters
{
    public class ParamContainer : IAlfrescoToolsParameters
    {
        public string ServerUrl => AlfrescoToolsTestContants.ServiceUrl;

        public string ALFRESCO_PATH => AlfrescoToolsTestContants.Alfresco_PATH;

        public string SAVING_PATH => AlfrescoToolsTestContants.SAVING_PATH;

        public string UserName => AlfrescoToolsTestContants.UserName;

        public string Password => AlfrescoToolsTestContants.Password;
    }
}
