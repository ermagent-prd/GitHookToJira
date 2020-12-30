using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace JiraToolsTest.Container
{
    internal static class ContainerForTest
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => ContainerFactory.Execute());
        #endregion

    }
}
