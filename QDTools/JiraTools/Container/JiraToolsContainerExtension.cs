using JiraTools.Engine;
using JiraTools.Service;
using Unity;
using Unity.Extension;

namespace JiraTools.Container
{
    public class JiraToolsContainerExtension : UnityContainerExtension
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        protected override void Initialize()
        {
            Container.RegisterType<JqlGetter>();

            Container.RegisterType<ServiceManagerContainer>();
            Container.RegisterType<AddWorklogEngine>();
            Container.RegisterType<AddCommentEngine>();
            Container.RegisterType<AddAttachmentEngine>();
            Container.RegisterType<UserGetter>();
            Container.RegisterType<UserListGetter>();
            Container.RegisterType<LinkEngine>();
            Container.RegisterType<ItemListGetter>();
        }

        #endregion

        #region Private methods


        #endregion

    }
}