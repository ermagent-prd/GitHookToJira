
using GeminiToJira.Parameters;
using JiraToExcel.Engines;
using JiraTools.Container;
using JiraTools.Parameters;
using Unity;

namespace JiraToExcel.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<JiraLoaderEngine>();

            container.RegisterType<IJiraToolsParameters, JiraParamContainer>();

            container.AddNewExtension<JiraToolsContainerExtension>();


            return container;
        }
    }
}
