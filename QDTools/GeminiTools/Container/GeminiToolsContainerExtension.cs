using GeminiTools.Engine;
using GeminiTools.Items;
using GeminiTools.Projects;
using GeminiTools.Service;
using Unity;
using Unity.Extension;

namespace GeminiTools.Container
{
    public class GeminiToolsContainerExtension : UnityContainerExtension
    {
        #region Public methods

        protected override void Initialize()
        {
            Container.RegisterType<ProjectFinder>();
            Container.RegisterType<ProjectGetter>();
            Container.RegisterType<ProjectListGetter>();

            Container.RegisterType<ItemGetter>();
            Container.RegisterType<ItemListGetter>();
            Container.RegisterType<LinkItemEngine>();
            Container.RegisterType<ServiceManagerContainer>();

            Container.RegisterType<AttachmentGetter>();

        }

        #endregion
    }
}