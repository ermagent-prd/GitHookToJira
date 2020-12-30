using JiraTools.Engine;
using Unity;
using Unity.Extension;

namespace JiraTools.Container
{
    public class ContainerExtension : UnityContainerExtension
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        protected override void Initialize()
        {
            Container.RegisterType<JiraRequestFactory>();
            Container.RegisterType<AddIssueEngine>();
            Container.RegisterType<AddIssueEngineTest>();

        }

        #endregion

        #region Private methods


        #endregion

    }
}