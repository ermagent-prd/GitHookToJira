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
    public class DocumentCreateTest
    {
        [TestMethod]
        public void Execute_CreateDocument()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<FolderCreateEngine>();

            var newFolder = engine.Execute("APi Test", "CreateTest", "");

            var engineDocument = container.Resolve<UploadDocumentEngine>();

            var path = engineDocument.Execute(newFolder, "prova.docx", AlfrescoToolsTestContants.AttachmentPath);

            Assert.IsNotNull(path);

        }

    }
}

