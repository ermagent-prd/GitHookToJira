using AlfrescoTools.Container;
using AlfrescoTools.Engine;
using AlfrescoTools.Parameters;
using GeminiToJira.Engine;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters;
using GeminiTools.Container;
using GeminiTools.Parameters;
using JiraTools.Container;
using JiraTools.Engine;
using JiraTools.Parameters;
using Unity;

namespace GeminiToJira.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IGeminiToolsParameters, GeminiParamContainer>();
            container.RegisterType<IJiraToolsParameters, JiraParamContainer>();
            container.RegisterType<IAlfrescoToolsParameters, AlfrescoParamContainer>();

            container.RegisterType<CommentMapper>();
            container.RegisterType<JiraAccountIdEngine>();

            container.RegisterType<ImportTaskEngine>();
            container.RegisterType<ImportStoryEngine>();
            container.RegisterType<ImportUatEngine>();
            container.RegisterType<ImportBugEngine>();
            container.RegisterType<TimeLogEngine>();

            container.RegisterType<FolderGetterEngine>();
            container.RegisterType<FolderCreateEngine>();
            container.RegisterType<UploadDocumentEngine>();

            container.RegisterType<ParseCommentEngine>();
            container.AddNewExtension<JiraToolsContainerExtension>();
            container.AddNewExtension<GeminiToolsContainerExtension>();
            container.AddNewExtension<AlfrescoToolsContainerExtension>();




            return container;
        }
    }
}
