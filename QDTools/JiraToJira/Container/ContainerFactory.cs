using GeminiToJira.Parameters;
using JiraToJira.Engine;
using JiraTools.Container;
using JiraTools.Parameters;
using Unity;

namespace JiraToJira.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<IJiraToolsParameters, JiraParamContainer>();

            container.RegisterType<ImportEngine>();
            container.RegisterType<CloneIssueEngine>();

            container.AddNewExtension<JiraToolsContainerExtension>();
            
            return container;
        }
    }
}
