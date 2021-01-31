
namespace AlfrescoTools.Parameters
{
    public interface IAlfrescoToolsParameters
    {
        string ServerUrl { get; }

        string ALFRESCO_PATH { get; }

        string SAVING_PATH { get; }

        string UserName { get; }

        string Password { get; }
    }
}
