using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using JiraTools.Parameters;
using GeminiTools.Parameters;
using AlfrescoTools.Parameters;


namespace GeminiToJiraTest
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
                },

                Mapping = new MappingConfiguration()
                {
                    AFFECTEDBUILD_LABEL = "AffectedBuild",
                    RELEASE_KEY_LABEL = "Release Version",
                    LINE_KEY_LABEL = "DVL",
                    
                    FUNCTIONALITY_LABEL = "Functionality",
                    RELATED_DEVELOPMENT_LABEL = "Development",
                    ISSUE_TYPE_LABEL = "IssueType",
                    FIXED_IN_BUILD_LABEL = "FixedInBuild",

                    BUG_FOUNDINBUILD_LABEL = "FoundInBuild",
                    BUG_PROJECT_MODULE = "Product Module",

                    DEV_STATUS_MAPPING = new Dictionary<string, string>()
                    {
                        { "backlog",        "Backlog" },
                        { "in Backlog",     "Backlog" },
                        { "assigned",       "Select for development" },
                        { "analysis",       "In Progress" },
                        { "development",    "In progress" },
                        { "waiting for test", "In progress" },
                        { "testing", "In progress" },
                        { "cancelled", "Done" },
                        { "done", "Done" },
                        { "in progress", "In progress" },
                    },
                    DEV_STATUS_MAPPING_DEFAULT = "Backlog",

                    DEV_ESTIMATE_TYPE_MAPPING = new Dictionary<string, string>()
                    {
                        { "planned", "Planned" },
                        { "cr-mkt", "Internal change request" },
                        { "cr-int",  "Market change request" },
                        { "overbudget", "Overbudget" },
                    },

                    DEV_ESTIMATE_TYPE_MAPPING_DEFAULT = "Planned",


                    TASK_TYPE_MAPPING = new Dictionary<string, string>()
                    {
                        { "analysis", "Analysis" },
                        { "development", "Development" },
                        { "test",       "Test" },
                        { "documentation",    "Documentation" },
                    },
                    TASK_TYPE_MAPPING_DEFAULT = "Other",

                    UAT_STATUS_MAPPING = new Dictionary<string, string>()
                    {
                        { "assigned",   "To Do" },
                        { "in progress",   "In progress" },
                        { "fixed",  "Ready for test" },
                        { "testing",   "Testing" },
                        { "closed",  "Closed" },
                        { "rejected",   "Rejected" },
                        { "cancelled",   "Cancelled" },
                        { "in Backlog",  "In Backlog" },
                    },
                    UAT_STATUS_MAPPING_DEFAULT = "To Do",

                    UAT_PRIORITY_MAPPING = new Dictionary<string, string>()
                    {
                        { "low", "Low" },
                        { "medium", "Medium" },
                        { "high", "High" },
                    },

                    UAT_SEVERITY_MAPPING = new Dictionary<string, string>()
                    {
                        { "low", "Trivial" },
                        { "medium", "Minor" },
                        { "high", "Major" },
                        { "highest", "Blocking" },
                    },

                    UAT_CATEGORY_MAPPING = new Dictionary<string, string>()
                    {
                        {"calcoli", "Functional"},
                        {"interfaccia", "Usability"},
                        {"usabilità", "Usability"},
                        {"efficenza", "Performance"},
                        {"suggerimento", "Usability"},
                        {"localizzazione", "Usability"},
                        {"messaggio di errore", "Functional"},
                        {"dal", "Functional"}
                    },
                    UAT_CATEGORY_MAPPING_DEFAULT = "Functional",


                    UAT_TYPE_MAPPING = new Dictionary<string, string>()
                    {
                        { "defect",   "Defect" },
                        { "investigation",   "Investigation" },
                        { "enhancement",   "Enhancement" },
                        { "enanchement",   "Enhancement" },
                        { "regression",   "Regression" },
                        { "setup",   "Setup" },
                        { "change request",   "Change request" },
                        { "new feature",   "New Feature" },
                        { "missing functionality",   "Missing Functionality" },
                    },

                    BUG_STATUS_MAPPING = new Dictionary<string, string>()
                    {
                        { "assigned", "Assigned" },
                        { "in progess", "In Progress" },
                        { "cancelled", "Won't Fix" },
                        { "testing", "In review" },
                        { "fixed", "Fixed" }
                    },

                    BUG_PRIORITY_MAPPING = new Dictionary<string, string>()
                    {
                        { "low", "Low" },
                        { "medium", "Medium" },
                        { "high", "High" },
                    },

                    BUG_TYPE_MAPPING = new Dictionary<string, string>()
                    {
                        { "Presentation", "Functional" },
                        { "Engine", "Functional" },
                        { "Sythesis", "Functional" },
                        { "Other", "Functional" },
                    },

                    BUG_CAUSE_TYPE_MAPPING = new Dictionary<string, string>()
                    {
                        {"development mistake","Development mistake" },
                        {"analysis mistake","Analysis mistake" },
                        {"sql mistake","Development mistake" },
                        {"presentation mistake","Presentation mistake" },
                        {"enhancement","Enhancement" },
                        {"missing development","Missing development" },
                        {"regression","Regression" },
                        {"refactoring","Refactoring" },
                        {"licensing","Licensing" },
                        {"complexity","Code complexity" },
                        {"db upgrade","Datasource upgrade" },
                        {"devops","Regression" },
                        {"duplicate code","Development mistake" },
                        {"performance","Performance" }
                    },

                    BUG_CAUSE_TYPE_DEFAULT = "Other",

                    BUG_SUGGESTED_ACTION_TYPE_MAPPING = new Dictionary<string, string>()
                    {
                        {"more attention", "Integration test" },
                        {"more test", "Integration test" },
                        {"unit test", "Test automation" },
                        {"tca", "Integration test" },
                        {"refactoring", "Refactoring" },
                        {"presentation test", "Integration test" },
                        {"missing documentation", "Documentation" },
                        {"uat", "Integration test" },
                        {"database test", "Integration test" },
                        {"enancement management", "Processes improvement" },
                        {"performance test", "Performance test" },
                        {"eagle Integration", "Integration test" },
                        {"licence test", "Licence test" },
                        {"use entity framework", "Refactoring" },
                        {"sql test", "Integration test" }
                    },

                    BUG_SUGGESTED_ACTION_TYPE_DEFAULT = "Other",
    }
            };

            // serialize JSON to a string and then write string to a file
            File.WriteAllText(@"C:\GeminiPorting\configuration.json", JsonConvert.SerializeObject(configuration, Formatting.Indented));

        }

        /*
        [TestMethod]
        public void SerializeExcelCOnfigurationTest()
        {
            var conf = new ExcelConfiguration()
            {
                JsqlQuery = "project = RMS5 AND type = Sub-task ORDER BY id ASC",
                ReportSheetName = "sheetName",
                ExcelFilePath = @"C:\Users\lucap\Desktop\Sviluppi\GeminiToJira\",
                ExcelFileName = "export.xlsx",
                FieldNames = new List<string>()
                {
                    "JiraCode",
                    "Summary",
                    "GeminiCode",
                    "Status",
                    "Original Estimate",
                    "Due Date",
                    "JDE Code",
                    "ParentCode"

                }
            };

            // serialize JSON to a string and then write string to a file
            File.WriteAllText(@"C:\GeminiPorting\exportconf.json", JsonConvert.SerializeObject(conf, Formatting.Indented));

        }
        */

        [TestMethod]
        public void DeSerializeConfigurationTest()
        {
            GeminiToJiraParameters configuration = JsonConvert.DeserializeObject<GeminiToJiraParameters>(File.ReadAllText(@"C:\GeminiPorting\configuration.json"));


            Assert.IsNotNull(configuration);
        }
    }
}
