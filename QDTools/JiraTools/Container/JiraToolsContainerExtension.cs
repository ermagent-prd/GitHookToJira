using JiraTools.Engine;
using JiraTools.Model;
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
            Container.RegisterType<IssueLinkSearchEngine>();
            Container.RegisterType<ItemListGetter>();
            Container.RegisterType<EditCustomFieldEngine>();
            Container.RegisterType<PublicAddCommentEngine>();
            Container.RegisterType<AddCustomFieldEngine>();
            Container.RegisterType<AddWatcherEngine>();
            Container.RegisterType<ProjectGetter>();
            Container.RegisterType<IProjectGetter, ProjectGetter>();
            Container.RegisterType<ProjectReleasesGetter>();
            Container.RegisterType<IProjectReleasesGetter,ProjectReleasesGetter>();
            Container.RegisterType<RemoteLinkEngine>();
        }

        #endregion

        #region Private methods


        #endregion

    }
}