using JiraTools.Container;
using System;
using Unity;

namespace JiraToolsTest.Container
{
    internal static class ContainerForTest
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => JiraContainerFactory.Execute());
        #endregion

    }
}
