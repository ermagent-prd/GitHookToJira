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
    public class FoldersGetterTest
    {
        [TestMethod]
        public void Execute_GetAllRootFolders()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<FolderGetterEngine>();

            var rootList = engine.Execute();

            foreach (var contentItem in rootList)
            {
                Console.WriteLine(contentItem.Name);
            }

            Assert.IsNotNull(rootList);

        }


        [TestMethod]
        public void Execute_GetSpecificFolder()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<FolderGetterEngine>();

            var folder = (Folder)engine.Execute("APi Test");
            
            Assert.IsNotNull(folder);

        }


    }
}

