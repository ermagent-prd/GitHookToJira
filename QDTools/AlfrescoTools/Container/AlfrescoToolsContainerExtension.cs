
using AlfrescoTools.Engine;
using AlfrescoTools.Service;
using Unity;
using Unity.Extension;

namespace AlfrescoTools.Container
{
    public class AlfrescoToolsContainerExtension : UnityContainerExtension
    {
        #region Public methods

        protected override void Initialize()
        {

            Container.RegisterType<FolderGetterEngine>();
            Container.RegisterType<FolderCreateEngine>();
            Container.RegisterType<UploadDocumentEngine>();

            Container.RegisterType<ServiceManagerContainer>();

        }

        #endregion
    }
}