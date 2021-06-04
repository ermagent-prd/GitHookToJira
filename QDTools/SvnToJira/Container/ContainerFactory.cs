using JiraTools.Container;
using JiraTools.Parameters;
using SvnToJira.Parameters;
using Unity;

namespace SvnToJira.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IJiraToolsParameters, JiraParamContainer>();
            container.AddNewExtension<JiraToolsContainerExtension>();

            return container;
        }
    }
}
