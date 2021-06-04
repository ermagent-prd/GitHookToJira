using GeminiToJira.Parameters;
using JiraReport.Engine;
using JiraTools.Container;
using JiraTools.Parameters;
using Unity;

namespace JiraReport.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<IJiraToolsParameters, JiraParamContainer>();

            container.RegisterType<ExportEngine>();
            container.RegisterType<ExportFieldsEngine>();
            container.RegisterType<RelatedDevEngine>();

            container.AddNewExtension<JiraToolsContainerExtension>();
            
            return container;
        }
    }
}
