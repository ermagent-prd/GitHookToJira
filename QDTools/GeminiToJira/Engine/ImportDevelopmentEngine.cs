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

namespace GeminiToJira.Engine
{
    public class ImportDevelopmentEngine
    {
        private readonly DevelopmentIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LinkItemEngine linkItemEngine;
        private readonly AttachmentGetter attachmentGetter;
        private readonly FolderCreateEngine folderEngine;
        private readonly UploadDocumentEngine uploadAlfrescoEngine;


        public ImportDevelopmentEngine(
            DevelopmentIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkItemEngine linkItemEngine,
            AttachmentGetter attachmentGetter,
            FolderCreateEngine folderEngine,
            UploadDocumentEngine uploadAlfrescoEngine
            )
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.linkItemEngine = linkItemEngine;
            this.attachmentGetter = attachmentGetter;
            this.folderEngine = folderEngine;
            this.uploadAlfrescoEngine = uploadAlfrescoEngine;

        }

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            var projectCode = configurationSetup.JiraProjectCode;
            
            var jiraSavedDictionary = new Dictionary<int, Issue>();
            var storyFolderDictionary = new Dictionary<string, string>();

            var storyType = configurationSetup.Jira.StoryTypeCode;
            var subTaskType = configurationSetup.Jira.SubTaskTypeCode;

            var filter = GetDevFilter(configurationSetup);
            var geminiDevelopmentIssueList = GetFilteredGeminiIssueList(geminiItemsEngine, filter, configurationSetup);

            var developmentLogFile = "DevelopmentLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            foreach (var geminiIssue in geminiDevelopmentIssueList.Where(l => l.Type == "Development").OrderBy(f => f.Id).ToList())
            {
                try
                {
                    var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                    var jiraIssueInfo = geminiToJiraMapper.Execute(configurationSetup, currentIssue, storyType, projectCode);

                    //Create Story
                    Issue jiraIssue = SaveAndSetStory(jiraSavedDictionary, geminiIssue, jiraIssueInfo, configurationSetup.AttachmentDownloadedPath);
                    var storyFolder = SetAndSaveAlfrescoUrls(jiraIssueInfo, jiraIssue, "", configurationSetup);
                    storyFolderDictionary.Add(jiraIssue.JiraIdentifier, storyFolder);

                    var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != configurationSetup.Gemini.GroupTypeCode && i.Value.Id != currentIssue.Id);

                    //SubTask
                    foreach (var sub in hierarchy)
                    {
                        if (sub.Value.Type == "Task")
                        {
                            var currentSubIssue = geminiItemsEngine.Execute(sub.Value.Id);

                            SaveAndSetSubTask(configurationSetup, jiraIssue, currentSubIssue, jiraSavedDictionary, subTaskType, storyFolder);
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(
                        configurationSetup.LogDirectory + developmentLogFile,
                        "[Task] - " + geminiIssue.IssueKey + " - " + ex.Message + Environment.NewLine);
                }
            }

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
                            Issue jiraIssue = SaveAndSetStory(jiraSavedDictionary, geminiIssue, jiraIssueInfo, configurationSetup.AttachmentDownloadedPath);
                            var storyFolder = SetAndSaveAlfrescoUrls(jiraIssueInfo, jiraIssue, "", configurationSetup);
                            storyFolderDictionary.Add(jiraIssue.JiraIdentifier, storyFolder);
                          
                            //and become a subtask of myself
                            SaveAndSetSubTask(configurationSetup, jiraIssue, currentSubIssue, jiraSavedDictionary, subTaskType, storyFolder);

                        }
                        else
                        {
                            //my father was inserted and i was not in his hierarchy list
                            var fatherKey = currentSubIssue.HierarchyKey.Split('|').ElementAt(1);

                            if (jiraSavedDictionary.TryGetValue(Convert.ToInt32(fatherKey), out Issue jiraFatherIssue))
                            {
                                storyFolderDictionary.TryGetValue(jiraFatherIssue.JiraIdentifier, out string storyFolder);

                                SaveAndSetSubTask(configurationSetup, jiraFatherIssue, currentSubIssue, jiraSavedDictionary, subTaskType, storyFolder);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(
                            configurationSetup.LogDirectory + developmentLogFile,
                            "[SubT] - " + currentSubIssue.IssueKey + " - " + ex.Message + Environment.NewLine);
                    }
                }
            }

            #endregion
        }

        private Issue SaveAndSetStory(Dictionary<int, Issue> jiraSavedDictionary, IssueDto geminiIssue, CreateIssueInfo jiraIssueInfo, string attachmentPath)
        {
            var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo, attachmentPath);
            //and set as saved
            if(!jiraSavedDictionary.TryGetValue(geminiIssue.Id, out Issue existing))
                jiraSavedDictionary.Add(geminiIssue.Id, jiraIssue);

            SetAndSaveReporter(jiraIssue, geminiIssue);
            return jiraIssue;
        }

        private void SaveAndSetSubTask(
            GeminiToJiraParameters configurationSetup,
            Issue jiraIssue,
            IssueDto currentSubIssue,
            Dictionary<int, Issue> jiraSavedDictionary,
            string subTaskType,
            string storyFolder)
        {
            if (CheckIfValidItem(currentSubIssue, configurationSetup))
            {
                var jiraSubTaskInfo = geminiToJiraMapper.Execute(configurationSetup, currentSubIssue, subTaskType, configurationSetup.JiraProjectCode);

                jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;

                //create subtask
                var subIssue = jiraSaveEngine.Execute(jiraSubTaskInfo, configurationSetup.AttachmentDownloadedPath);
                //and set as saved
                if (!jiraSavedDictionary.TryGetValue(currentSubIssue.Id, out Issue existing))
                    jiraSavedDictionary.Add(currentSubIssue.Id, subIssue);

                SetAndSaveReporter(subIssue, currentSubIssue);
                SetAndSaveAlfrescoUrls(jiraSubTaskInfo, subIssue, storyFolder, configurationSetup);

            }
        }



        #region Private 
        /// <summary>
        /// The subtask must have the same release and development lines of his father (story)
        /// </summary>
        /// <param name="geminiIssue"></param>
        /// <returns></returns>
        private bool CheckIfValidItem(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Filter.DEVELOPMENT_RELEASE_KEY);
            var devLine = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Filter.DEVELOPMENT_LINE_KEY);

            //Solo i development: i task sono quelli associati ai development trovati
            if (release != null && devLine != null &&
                configurationSetup.Filter.DEVELOPMENT_RELEASES.Contains(release.FormattedData) &&
                configurationSetup.Filter.DEVELOPMENT_LINES.Contains(devLine.FormattedData))
                return true;
            else
                return false;

        }

        private IEnumerable<IssueDto> GetFilteredGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            Countersoft.Gemini.Commons.Entity.IssuesFilter filter,
            GeminiToJiraParameters configurationSetup)
        {
            var geminiIssueList = geminiItemsEngine.Execute(filter);

            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in geminiIssueList.OrderBy(f => f.Id))
            {
                if(CheckIfValidItem(l, configurationSetup))
                    filteredList.Add(l);
            }

            return filteredList;
        }

        private Countersoft.Gemini.Commons.Entity.IssuesFilter GetDevFilter(GeminiToJiraParameters configurationSetup)
        {
            return new Countersoft.Gemini.Commons.Entity.IssuesFilter
            {
                IncludeClosed = configurationSetup.Filter.DEVELOPMENT_INCLUDED_CLOSED,
                Projects = configurationSetup.Filter.DEVELOPMENT_PROJECT_ID,
                Types = configurationSetup.Filter.DEVELOPMENT_TYPES,
                Issues = "|59721|"
            };
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter).AccountId;
                jiraIssue.SaveChanges();
            }
        }

        private string SetAndSaveAlfrescoUrls(
            CreateIssueInfo jiraIssueInfo, 
            Issue jiraIssue, 
            string storyFolder, 
            GeminiToJiraParameters configurationSetup)
        {
            LinkItem analysisLink = null;
            LinkItem brAnalysisLink = null;
            LinkItem changeDocumentLink = null;
            LinkItem testDocumentLink = null;

            GetLinks(jiraIssueInfo, ref analysisLink, ref brAnalysisLink, ref changeDocumentLink, ref testDocumentLink);

            //if all links are anull no url to save
            if (analysisLink == null && 
                brAnalysisLink == null && 
                changeDocumentLink == null && 
                testDocumentLink == null)
                return "";

            var newFolder = jiraIssue.Key.Value + " " + jiraIssue.Summary;
            IFolder folderAlfresco = null;

            //new folder is needed only if there is , at least, one File to save (url don't need folders)
            if((analysisLink != null && analysisLink.FileName != "") ||
                (brAnalysisLink != null && brAnalysisLink.FileName != "") ||
                (changeDocumentLink != null && changeDocumentLink.FileName != "") ||
                (testDocumentLink != null && testDocumentLink.FileName != ""))
                folderAlfresco = folderEngine.Execute(configurationSetup.Alfresco.RootFolder, newFolder, storyFolder);

            if (analysisLink != null)
                jiraIssue.CustomFields.Add("Analysis Document Url", SaveAndUploadToAlfresco(folderAlfresco, analysisLink, configurationSetup));

            if (brAnalysisLink != null)
                jiraIssue.CustomFields.Add("BR Analysis Url", SaveAndUploadToAlfresco(folderAlfresco, brAnalysisLink, configurationSetup));

            if (changeDocumentLink != null)
                jiraIssue.CustomFields.Add("Change Document Url", SaveAndUploadToAlfresco(folderAlfresco, changeDocumentLink, configurationSetup));

            if (testDocumentLink != null)
                jiraIssue.CustomFields.Add("Test Document Url", SaveAndUploadToAlfresco(folderAlfresco, testDocumentLink, configurationSetup));

            jiraIssue.SaveChanges();

            return newFolder;
        }

        private void GetLinks(CreateIssueInfo jiraIssueInfo, 
            ref LinkItem analysisLink, 
            ref LinkItem brAnalysisLink, 
            ref LinkItem changeDocumentLink, 
            ref LinkItem testDocumentLink)
        {
            if (jiraIssueInfo.AnalysisUrl != null && jiraIssueInfo.AnalysisUrl != "")
                analysisLink = GetAttachmentLinkItem(jiraIssueInfo.AnalysisUrl);

            if (jiraIssueInfo.BrAnalysisUrl != null && jiraIssueInfo.BrAnalysisUrl != "")
                brAnalysisLink = GetAttachmentLinkItem(jiraIssueInfo.BrAnalysisUrl);

            if (jiraIssueInfo.ChangeDocumentUrl != null && jiraIssueInfo.ChangeDocumentUrl != "")
                changeDocumentLink = GetAttachmentLinkItem(jiraIssueInfo.ChangeDocumentUrl);

            if (jiraIssueInfo.TestDocumentUrl != null && jiraIssueInfo.TestDocumentUrl != "")
                testDocumentLink = GetAttachmentLinkItem(jiraIssueInfo.TestDocumentUrl);
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
