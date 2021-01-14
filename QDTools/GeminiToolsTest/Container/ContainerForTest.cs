using GeminiTools.Container;
using System;
using Unity;

namespace GeminiToolsTest.Container
{
    internal static class ContainerForTest
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => GeminiContainerFactory.Execute());
        #endregion

    }
}
