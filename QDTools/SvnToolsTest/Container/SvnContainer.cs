using System;
using Unity;

namespace SvnToolsTest.Container
{
    public static class SvnContainer
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => SvnContainerFactory.Execute());
        #endregion

    }
}
