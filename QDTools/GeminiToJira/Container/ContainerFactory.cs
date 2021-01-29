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
            container.RegisterType<CommentMapper>();
            container.RegisterType<JiraAccountIdEngine>();

            container.RegisterType<ImportDevelopmentEngine>();
            container.RegisterType<ImportUatEngine>();
            container.RegisterType<ImportBugEngine>();
            
            container.RegisterType<ParseCommentEngine>();
            container.AddNewExtension<JiraToolsContainerExtension>();
            container.AddNewExtension<GeminiToolsContainerExtension>();


            return container;
        }
    }
}
