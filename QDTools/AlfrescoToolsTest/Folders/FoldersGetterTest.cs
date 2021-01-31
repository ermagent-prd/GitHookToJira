using AlfrescoToolsTest.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.IO;
using System;
using AlfrescoToolsTest.Container;
using AlfrescoTools.Engine;
using Unity;

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

            var root = engine.Execute();


        }

        [TestMethod]
        public void TestMethod2()
        {
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StreamContent(File.Open(AlfrescoToolsTestContants.AttachmentPath + "prova.txt", FileMode.Open)), "filedata", "prova.txt");
                    formData.Add(new StringContent("mysiteid"), "siteid");
                    formData.Add(new StringContent("mycontainerid"), "containerid");
                    formData.Add(new StringContent("/"), "uploaddirectory");
                    formData.Add(new StringContent("test"), "description");
                    formData.Add(new StringContent("cm:content"), "contenttype");
                    formData.Add(new StringContent("true"), "overwrite");

                    //http://10.100.2.85:8080/alfresco/api/-default-/public/cmis/versions/1.1/atom
                    var response = client.PostAsync("http://10.100.2.85:8080/alfresco/service/api/upload?alf_ticket=TICKET_XXXXXXXXXXXXXXXXXXXXXXXXX", formData).Result;

                    string result = null;
                    if (response.Content != null)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        if (string.IsNullOrWhiteSpace(result))
                            result = "Upload successful!";
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(result))
                            result = "Upload failed for unknown reason";
                    }

                    Console.WriteLine($"Result is: {result}");
                }
            }

        }
    }
}
