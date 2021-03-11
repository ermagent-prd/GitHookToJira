using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeminiToJira.Parameters.Import;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using JiraTools.Parameters;
using GeminiTools.Parameters;
using AlfrescoTools.Parameters;

namespace ImportConfigurationTest
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void SerializeConfigurationTest()
        {
            var configuration = new GeminiToJiraParameters()
            {
                JiraProjectCode = "EIB",

                ComponentsForDevelopment = new List<String> { "ILIAS", "ILIAS-STA", "BSM", "Other" },

                AttachmentDownloadedPath = @"C:\GeminiPorting\AttachmentDownloaded\",
                LogDirectory = @"C:\GeminiPorting\Log\",

                Filter = new FilterConfiguration()
                {
                    STORY_PROJECT_ID = "|36|",
                    STORY_INCLUDED_CLOSED = true,
                    UAT_FUNCTIONALITY = new List<String> { "PYTHO" },
                    UAT_PROJECT_ID = "|37|",
                    UAT_CREATED_FROM = "8/27/2020",
                    UAT_GROUP_DEPENDENCIES = true,
                    UAT_INCLUDED_CLOSED = true,
                    ERMBUG_PROJECT_ID = "38",
                    ERMBUG_INCLUDED_CLOSED = true,

                    STORY_TYPES = "|Developer|Task|",

                    STORY_RELEASES = new List<string>() {
                    "ERMAS",
                    "ERMAS 5.24.0",
                    "ERMAS 5.24.1",
                    "ERMAS 5.25.0",
                    "ERMAS 5.26.0",
                    "ERMAS 5.27.0",
                    "ERMAS 5.28.0",
                    "ERMAS 5.29.0",
                    "0.0.0.0"
                    },

                    STORY_LINES = new List<string> {
                    "BSM",
                    "ILIAS",
                    "ILIAS-STA",
                    "Other"
                    },

                    STORY_RELEASE_KEY = "Release Version",
                    STORY_LINE_KEY = "DVL",
                },
                Alfresco = new AlfrescoConfiguration()
                {


                    ServiceUrl = "http://10.100.2.85:8080/alfresco/cmisatom",
                    RootFolder = "APi Test",

                    UserName = "apiuser",
                    Password = "cmis",
                },
                Gemini = new GeminiConfiguration()
                {
                    Url = "https://erm-swfactory.prometeia.com/Gemini",
                    ErmBugProjectCode = "ERMBUG",
                    ErmPrefix = "ERM-",
                    UatPrefix = "UAT-",
                    ErmBugPrefix = "ERMBUG-",
                    ProjectUrl = "https://erm-swfactory.prometeia.com/Gemini/project/",
                    GroupTypeCode = "Group"

                },
                Jira = new JiraConfiguration()
                {

                    User = "pierluigi.nanni@prometeia.com", //"paolo.luca@prometeia.com";
                    Token = "GOojveJqyGUAgi6mzSAu3FAA", //"1l9yaa74u6wqg4lbhitOE7C7";
                    Url = "https://prometeia-erm.atlassian.net/",
                    IssueApi = "rest/api/3/issue/",
                    MaxIssuesPerRequest = 10, //Max 100 ??

                    ImportTask = false,
                    ImportStory = true,
                    ImportBug = true,
                    ImportUat = true,
                    EpicTypeCode = "10000",
                    StoryTypeCode = "10001",
                    StorySubTaskTypeCode = "10017",
                    TaskTypeCode = "10002",
                    SubTaskTypeCode = "10003",
                    UatTypeCode = "10014",
                    BugTypeCode = "10004",
                }
            };

            // serialize JSON to a string and then write string to a file
            File.WriteAllText(@"C:\GeminiPorting\configuration.json", JsonConvert.SerializeObject(configuration, Formatting.Indented));
            
        }

        [TestMethod]
        public void DeSerializeConfigurationTest()
        {
            GeminiToJiraParameters configuration = JsonConvert.DeserializeObject<GeminiToJiraParameters>(File.ReadAllText(@"C:\GeminiPorting\configuration.json"));


            Assert.IsNotNull(configuration);


        }
    }
}
