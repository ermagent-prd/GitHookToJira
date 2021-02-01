using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS;
using System.Net.Http;
using AlfrescoToolsTest.Parameters;
using System.IO;

namespace AlfrescoToolsTest.Connecting
{
    /// <summary>
    /// Summary description for ConnectTest
    /// </summary>
    [TestClass]
    public class ConnectTest
    {
        [TestMethod]
        public void TestConnection()
        {
            SessionFactory sessionFactory = SessionFactory.NewInstance();
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add(SessionParameter.User, "apiuser");
            parameters.Add(SessionParameter.Password, "cmis");
            parameters.Add(SessionParameter.AtomPubUrl, "http://10.100.2.85:8080/alfresco/cmisatom");
            parameters.Add(SessionParameter.BindingType, BindingType.AtomPub);
            parameters.Add(SessionParameter.Compression, "true");
            parameters.Add(SessionParameter.CacheTTLObjects, "0");

            // If there is only one repository exposed (e.g. Alfresco),
            // these lines will help detect it and its ID
            var repositories = sessionFactory.GetRepositories(parameters);
            IRepository alfrescoRepository = null;
            if (repositories != null && repositories.Count > 0)
            {
                Console.WriteLine("Found (" + repositories.Count + ") Alfresco repositories");
                alfrescoRepository = repositories[0]; Console.WriteLine("Info about the first Alfresco repo [ID=" +
         alfrescoRepository.Id + "][name=" +
         alfrescoRepository.Name + "][CMIS ver supported=" +
         alfrescoRepository.CmisVersionSupported + "]");
            }
            else
            {
                throw new Exception("Could not connect to the Alfresco Server, " +
                                "no repository found!");
            }

            // Create a new session with the Alfresco repository
            var session = alfrescoRepository.CreateSession();

            Assert.IsNotNull(session);

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
