using SvnTools.Engine;
using Unity;
using Unity.Extension;

namespace SvnTools.Container
{
    public class SvnToolsContainerExtension : UnityContainerExtension
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        protected override void Initialize()
        {
            Container.RegisterType<RevisionPropertiesEngine>();
            Container.RegisterType<TrackingIssueGetter>();
            Container.RegisterType<SvnLookEngine>();

        }

        #endregion

        #region Private methods


        #endregion

    }
}