using AlfrescoToolsTest.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.IO;
using System;
using AlfrescoToolsTest.Container;
using AlfrescoTools.Engine;
using Unity;
using DotCMIS.Client.Impl;
using DotCMIS;
using System.Collections.Generic;
using DotCMIS.Client;

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

            var newFolder = engine.Execute("APi Test", "CreateTest");
            
            Assert.IsNotNull(newFolder);

        }

    }
}

