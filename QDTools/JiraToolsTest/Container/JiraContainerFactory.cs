using JiraTools.Container;
using JiraTools.Parameters;
using JiraToolsTest.Parameters;
using Unity;

namespace JiraToolsTest.Container
{
    internal static class JiraContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IJiraToolsParameters, ParamContainer>();
            container.AddNewExtension<JiraToolsContainerExtension>();
            return container;
        }
    }
}
