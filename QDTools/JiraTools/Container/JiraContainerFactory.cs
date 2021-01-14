using JiraTools.Container;
using JiraTools.Parameters;
using Unity;

namespace JiraTools.Container
{
    internal static class JiraContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IJiraToolsParameters, ParamContainer>();
            container.AddNewExtension<ContainerExtension>();
            return container;
        }
    }
}
