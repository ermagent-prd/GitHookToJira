using GeminiTools.Parameters;
using Unity;

namespace GeminiTools.Container
{
    public static class GeminiContainerFactory
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
