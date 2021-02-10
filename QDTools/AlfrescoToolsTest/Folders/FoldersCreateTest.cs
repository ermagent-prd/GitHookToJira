using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlfrescoToolsTest.Container;
using AlfrescoTools.Engine;
using Unity;

namespace AlfrescoToolsTest
{
    [TestClass]
    public class FoldersCreateTest
    {
        [TestMethod]
        public void Execute_CreateFolder()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<FolderCreateEngine>();

            var newFolder = engine.Execute("APi Test", "CreateTest", "");
            
            Assert.IsNotNull(newFolder);

        }

        [TestMethod]
        public void Execute_CreateSubFolder()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<FolderCreateEngine>();

            var newFolder = engine.Execute("APi Test", "Subfolder", "CreateTest");

            Assert.IsNotNull(newFolder);

        }

    }
}

