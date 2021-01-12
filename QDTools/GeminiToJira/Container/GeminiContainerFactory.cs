using GeminiTools.Container;
using GeminiTools.Parameters;
using Unity;

namespace GeminiToJira.Container
{
    internal static class GeminiContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IGeminiToolsParameters, ParamContainer>();
            container.AddNewExtension<ContainerExtension>();
            return container;
        }
    }
}
