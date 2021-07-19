using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Mapper;
using JiraTools.Engine;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using System;
using System.IO;
using GeminiTools.Engine;
using JiraTools.Model;
using GeminiTools.Items;
using AlfrescoTools.Engine;
using DotCMIS.Client;
using GeminiToJira.Parameters.Import;
using JiraTools.Parameters;
using GeminiToJira.Log;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GeminiToJira.Engine
{
    public class ImportStoryEngine
    {
        private readonly StoryIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LinkItemEngine linkItemEngine;
        private readonly AttachmentGetter attachmentGetter;
        private readonly FolderCreateEngine folderEngine;
        private readonly UploadDocumentEngine uploadAlfrescoEngine;

        private readonly FilteredGeminiIssueListGetter issueGetter;
        private readonly GeminiIssueChecker issueChecker;

        private readonly LogManager logManager;


        public ImportStoryEngine(
            StoryIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkItemEngine linkItemEngine,
            AttachmentGetter attachmentGetter,
            FolderCreateEngine folderEngine,
            UploadDocumentEngine uploadAlfrescoEngine,
            FilteredGeminiIssueListGetter issueGetter,
            GeminiIssueChecker issueChecker,
            LogManager logManager)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.linkItemEngine = linkItemEngine;
            this.attachmentGetter = attachmentGetter;
            this.folderEngine = folderEngine;
            this.uploadAlfrescoEngine = uploadAlfrescoEngine;
            this.issueGetter = issueGetter;
            this.issueChecker = issueChecker;
            this.logManager = logManager;
        }

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            var projectCode = configurationSetup.JiraProjectCode;

            var jiraSavedDictionary = new Dictionary<int, Issue>();
            var storyFolderDictionary = new Dictionary<string, string>();

            var storyType = configurationSetup.Jira.StoryTypeCode;
            var storySubTaskType = configurationSetup.Jira.SubTaskTypeCode;

            var geminiDevelopmentIssueList = this.issueGetter.Execute(configurationSetup);

            
            var storyLogFile = "DevelopmentLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            this.logManager.SetLogFile(configurationSetup.LogDirectory + storyLogFile);

            var filteredDevelopments = 
                geminiDevelopmentIssueList.Where(l => l.Type == "Development" || l.Type == "Enhancement")
                .OrderBy(f => f.Id).ToList();

            foreach (var geminiIssue in filteredDevelopments)
            {
                importStory(
                    configurationSetup, 
                    projectCode, 
                    jiraSavedDictionary, 
                    storyFolderDictionary, 
                    storyType, 
                    storySubTaskType, 
                    filteredDevelopments, 
                    geminiIssue);
            }

            /*
            Parallel.ForEach(filteredDevelopments, geminiIssue =>
            {
                importStory(
                     configurationSetup,
                     projectCode,
                     jiraSavedDictionary,
                     storyFolderDictionary,
                     storyType,
                     storySubTaskType,
                     filteredDevelopments,
                     geminiIssue);
            });
            */

            //orphansManagement(configurationSetup, projectCode, jiraSavedDictionary, storyFolderDictionary, storyType, storySubTaskType, geminiDevelopmentIssueList, storyLogFile);
        }

        private void importStory(
            GeminiToJiraParameters configurationSetup, 
            string projectCode,
            Dictionary<int, Issue> jiraSavedDictionary, 
            Dictionary<string, string> storyFolderDictionary, 
            string storyType, 
            string storySubTaskType, 
            List<IssueDto> filteredDevelopments, 
            IssueDto geminiIssue)
        {
            try
            {
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                var jiraIssueInfo = geminiToJiraMapper.Execute(configurationSetup, currentIssue, storyType, projectCode);

                //Create Story
                Issue jiraIssue = SaveAndSetStory(jiraSavedDictionary, geminiIssue, jiraIssueInfo, configurationSetup);
                var storyFolder = SetAndSaveAlfrescoUrls(jiraIssueInfo, jiraIssue, "", configurationSetup);
                storyFolderDictionary.Add(jiraIssue.JiraIdentifier, storyFolder);

                var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != configurationSetup.Gemini.GroupTypeCode && i.Value.Id != currentIssue.Id);

                //Story-SubTask
                foreach (var sub in hierarchy)
                {
                    if (sub.Value.Type == "Task")
                    {
                        //Esclude figli di development (i dev sono censiti come story non in dipendenza
                        if (sub.Value.ParentIssueId.HasValue && sub.Value.ParentIssueId.Value != currentIssue.Id)
                        {
                            var devParentIssue = filteredDevelopments.FirstOrDefault(d => d.Id == sub.Value.ParentIssueId.Value);

                            if (devParentIssue != null)
                                continue;
                        }

                        var currentSubIssue = geminiItemsEngine.Execute(sub.Value.Id);

                        SaveAndSetStorySubTask(configurationSetup, jiraIssue, currentSubIssue, jiraSavedDictionary, storySubTaskType, storyFolder, checkRelease: false);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logManager.Execute(
                    "[Story] - " + geminiIssue.IssueKey + " - " + ex.Message);
            }
        }





        #region Private 

        private void orphansManagement(GeminiToJiraParameters configurationSetup, string projectCode, Dictionary<int, Issue> jiraSavedDictionary, Dictionary<string, string> storyFolderDictionary, string storyType, string storySubTaskType, IEnumerable<IssueDto> geminiDevelopmentIssueList, string storyLogFile)
        {
            #region Orphans
            //manage orphans if exist (an orphan is a gemini task that was not listed in the hierarhy of a Story, that was previous inserted
            foreach (var geminiIssue in geminiDevelopmentIssueList.Where(l => l.Type == "Task").OrderBy(f => f.Id).ToList())
            {
                if (!jiraSavedDictionary.TryGetValue(geminiIssue.Id, out Issue jiraIssueSaved))
                {
                    var currentSubIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                    try
                    {
                        //if have no father
                        if (currentSubIssue.HierarchyKey == "")
                        {
                            //Create Story
                            var jiraIssueInfo = geminiToJiraMapper.Execute(configurationSetup, currentSubIssue, storyType, projectCode);
                            Issue jiraIssue = SaveAndSetStory(jiraSavedDictionary, geminiIssue, jiraIssueInfo, configurationSetup);
                            var storyFolder = SetAndSaveAlfrescoUrls(jiraIssueInfo, jiraIssue, "", configurationSetup);
                            storyFolderDictionary.Add(jiraIssue.JiraIdentifier, storyFolder);

                            //and become a subtask of myself
                            SaveAndSetStorySubTask(configurationSetup, jiraIssue, currentSubIssue, jiraSavedDictionary, storySubTaskType, storyFolder, checkRelease: true);

                        }
                        else
                        {
                            //my father was inserted and i was not in his hierarchy list
                            var fatherKey = currentSubIssue.HierarchyKey.Split('|').ElementAt(1);

                            if (jiraSavedDictionary.TryGetValue(Convert.ToInt32(fatherKey), out Issue jiraFatherIssue))
                            {
                                storyFolderDictionary.TryGetValue(jiraFatherIssue.JiraIdentifier, out string storyFolder);

                                SaveAndSetStorySubTask(configurationSetup, jiraFatherIssue, currentSubIssue, jiraSavedDictionary, storySubTaskType, storyFolder, checkRelease: true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logManager.Execute("[SubStory] - " + currentSubIssue.IssueKey + " - " + ex.Message);
                    }
                }
            }

            #endregion
        }


        private Issue SaveAndSetStory(Dictionary<int, Issue> jiraSavedDictionary, IssueDto geminiIssue, CreateIssueInfo jiraIssueInfo, GeminiToJiraParameters configurationSetup)
        {
            //save story
            var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo, configurationSetup.Jira, configurationSetup.AttachmentDownloadedPath);
            //and set as saved
            if (!jiraSavedDictionary.TryGetValue(geminiIssue.Id, out Issue existing))
                jiraSavedDictionary.Add(geminiIssue.Id, jiraIssue);

            //save reporter
            SetAndSaveReporter(jiraIssue, geminiIssue, configurationSetup.Jira.DefaultAccount);
            return jiraIssue;
        }

        private void SaveAndSetStorySubTask(
            GeminiToJiraParameters configurationSetup,
            Issue jiraIssue,
            IssueDto currentSubIssue,
            Dictionary<int, Issue> jiraSavedDictionary,
            string storySubTaskType,
            string rooStoryFolder,
            bool checkRelease)
        {
            if (checkRelease && !this.issueChecker.Execute(currentSubIssue, configurationSetup))
                return;

            var jiraStorySubTaskInfo = geminiToJiraMapper.Execute(configurationSetup, currentSubIssue, storySubTaskType, configurationSetup.JiraProjectCode);

            jiraStorySubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;

            //create subtask
            var subIssue = jiraSaveEngine.Execute(jiraStorySubTaskInfo, configurationSetup.Jira, configurationSetup.AttachmentDownloadedPath);
            //and set as saved
            if (!jiraSavedDictionary.TryGetValue(currentSubIssue.Id, out Issue existing))
                jiraSavedDictionary.Add(currentSubIssue.Id, subIssue);

            SetAndSaveReporter(subIssue, currentSubIssue,configurationSetup.Jira.DefaultAccount);
            SetAndSaveAlfrescoUrls(jiraStorySubTaskInfo, subIssue, rooStoryFolder, configurationSetup);
        }


        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue,string accountdefault)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter, accountdefault).AccountId;
                jiraIssue.SaveChanges();
            }
        }

        private string SetAndSaveAlfrescoUrls(
            CreateIssueInfo jiraIssueInfo, 
            Issue jiraIssue, 
            string rooStoryFolder, 
            GeminiToJiraParameters configurationSetup)
        {
            LinkItem analysisLink = null;
            LinkItem brAnalysisLink = null;
            LinkItem changeDocumentLink = null;
            LinkItem testDocumentLink = null;
            LinkItem newFeatureDocumentUrl = null;

            GetLinks(jiraIssueInfo, ref analysisLink, ref brAnalysisLink, ref changeDocumentLink, ref testDocumentLink, ref newFeatureDocumentUrl);

            //if all links are anull no url to save
            if (analysisLink == null && 
                brAnalysisLink == null && 
                changeDocumentLink == null && 
                testDocumentLink == null &&
                newFeatureDocumentUrl == null)
                return "";

            var newFolder = jiraIssue.Key.Value + " " + jiraIssue.Summary;
            IFolder folderAlfresco = null;

            //new folder is needed only if there is , at least, one File to save (url don't need folders)
            if((analysisLink != null && analysisLink.FileName != "") ||
                (brAnalysisLink != null && brAnalysisLink.FileName != "") ||
                (changeDocumentLink != null && changeDocumentLink.FileName != "") ||
                (testDocumentLink != null && testDocumentLink.FileName != "") ||
                (newFeatureDocumentUrl != null && newFeatureDocumentUrl.FileName != "") 
                )
                folderAlfresco = folderEngine.Execute(configurationSetup.Alfresco.RootFolder, newFolder, rooStoryFolder);

            if (analysisLink != null)
                SaveAndUploadToAlfresco(folderAlfresco, analysisLink, configurationSetup);

            if (brAnalysisLink != null)
                SaveAndUploadToAlfresco(folderAlfresco, brAnalysisLink, configurationSetup);

            if (changeDocumentLink != null)
                SaveAndUploadToAlfresco(folderAlfresco, changeDocumentLink, configurationSetup);

            if (testDocumentLink != null)
                SaveAndUploadToAlfresco(folderAlfresco, testDocumentLink, configurationSetup);

            if (newFeatureDocumentUrl != null)
                SaveAndUploadToAlfresco(folderAlfresco, newFeatureDocumentUrl, configurationSetup);

            //TODOPL save alfresco folder in csustomField if and only if folderAlfresco != null
            if (folderAlfresco != null && jiraIssue.Type.Id == configurationSetup.Jira.StoryTypeCode)
            {
                jiraIssue.CustomFields.Add("Documentation Url", "http://10.100.2.85:8080/share/page/folder-details?nodeRef="+folderAlfresco.Id);
                
                jiraIssue.SaveChanges();
            }

            return newFolder;
        }

        private void GetLinks(CreateIssueInfo jiraIssueInfo, 
            ref LinkItem analysisLink, 
            ref LinkItem brAnalysisLink, 
            ref LinkItem changeDocumentLink, 
            ref LinkItem testDocumentLink,
            ref LinkItem newFeatureDocumentUrl)
        {
            if (jiraIssueInfo.AnalysisUrl != null && jiraIssueInfo.AnalysisUrl != "")
                analysisLink = GetAttachmentLinkItem(jiraIssueInfo.AnalysisUrl);

            if (jiraIssueInfo.BrAnalysisUrl != null && jiraIssueInfo.BrAnalysisUrl != "")
                brAnalysisLink = GetAttachmentLinkItem(jiraIssueInfo.BrAnalysisUrl);

            if (jiraIssueInfo.ChangeDocumentUrl != null && jiraIssueInfo.ChangeDocumentUrl != "")
                changeDocumentLink = GetAttachmentLinkItem(jiraIssueInfo.ChangeDocumentUrl);

            if (jiraIssueInfo.TestDocumentUrl != null && jiraIssueInfo.TestDocumentUrl != "")
                testDocumentLink = GetAttachmentLinkItem(jiraIssueInfo.TestDocumentUrl);

            if (jiraIssueInfo.NewFeatureDocumentUrl != null && jiraIssueInfo.NewFeatureDocumentUrl != "")
                newFeatureDocumentUrl = GetAttachmentLinkItem(jiraIssueInfo.NewFeatureDocumentUrl);
        }

        private LinkItem GetAttachmentLinkItem(string attachmentUrl)
        {
            if (attachmentUrl.Contains("drive.google.com"))
                return new LinkItem
                {
                    Href = attachmentUrl.Replace("<p>", "").Replace("</p>", ""),
                    FileName = ""
                };
            else
            {
                return linkItemEngine.Execute(attachmentUrl);
            }
        }

        private string SaveAndUploadToAlfresco(IFolder folderAlfresco, LinkItem attachmentLink, GeminiToJiraParameters configurationSetup)
        {
            string url = "";

            if (attachmentLink == null)
                return url;

            if (attachmentLink.FileName == "")
                return attachmentLink.Href;

            attachmentGetter.Save(attachmentLink, configurationSetup.AttachmentDownloadedPath);

            url = uploadAlfrescoEngine.Execute(folderAlfresco, attachmentLink.FileName, configurationSetup.AttachmentDownloadedPath);

            if (File.Exists(configurationSetup.AttachmentDownloadedPath + attachmentLink.FileName))
                File.Delete(configurationSetup.AttachmentDownloadedPath + attachmentLink.FileName);

            return url;

        }
        #endregion
    }


}
