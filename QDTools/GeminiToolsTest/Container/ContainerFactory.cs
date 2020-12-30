using GeminiTools.Container;
using GeminiTools.Parameters;
using GeminiToolsTest.Parameters;
using Unity;

namespace GeminiToolsTest.Container
{
    internal static class ContainerFactory
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
