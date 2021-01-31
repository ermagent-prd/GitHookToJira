using System;
using Unity;

namespace AlfrescoToolsTest.Container
{
    internal static class ContainerForTest
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => AlfrescoContainerFactory.Execute());
        #endregion

    }
}
