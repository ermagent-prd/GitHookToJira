using GeminiTools.Items;
using GeminiTools.Projects;
using GeminiTools.Service;
using Unity;
using Unity.Extension;

namespace GeminiTools.Container
{
    public class ContainerExtension : UnityContainerExtension
    {
        #region Public methods

        protected override void Initialize()
        {
            Container.RegisterType<ProjectFinder>();
            Container.RegisterType<ProjectGetter>();
            Container.RegisterType<ProjectListGetter>();

            Container.RegisterType<ItemGetter>();
            Container.RegisterType<ItemListGetter>();
            Container.RegisterType<ServiceManagerContainer>();

        }

        #endregion
    }
}