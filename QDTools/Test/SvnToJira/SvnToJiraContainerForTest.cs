using SvnToJira.Parameters;
using System;
using Unity;

namespace SvnToJiraTest
{
    internal static class SvnToJiraContainerForTest
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => SvnToJira.Container.ContainerFactory.Execute(new SvnToJiraParameters()));
        #endregion

    }
}
