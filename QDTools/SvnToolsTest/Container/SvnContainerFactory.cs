using SvnTools.Container;
using SvnTools.Parameters;
using SvnToolsTest.Parameters;
using Unity;

namespace SvnToolsTest.Container
{
    internal static class SvnContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<ISvnToolsParameters, ParamContainer>();
            container.AddNewExtension<SvnToolsContainerExtension>();
            return container;
        }
    }
}
