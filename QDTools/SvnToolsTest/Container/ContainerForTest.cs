using System;
using Unity;

namespace SvnToolsTest.Container
{
    internal static class ContainerForTest
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => SvnContainerFactory.Execute());
        #endregion

    }
}
