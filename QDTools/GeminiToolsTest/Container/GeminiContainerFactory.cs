using GeminiTools.Container;
using GeminiTools.Parameters;
using GeminiToolsTest.Parameters;
using Unity;

namespace GeminiToolsTest.Container
{
    public static class GeminiContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IGeminiToolsParameters, ParamContainer>();
            container.AddNewExtension<GeminiToolsContainerExtension>();
            return container;
        }
    }
}
