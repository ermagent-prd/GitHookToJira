using System;
using Unity;

namespace JiraToolsTest.Container
{
    public static class JiraContainer
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => JiraContainerFactory.Execute());
        #endregion

    }
}
