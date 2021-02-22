using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.GeminiFilter;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters;
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

        public void Execute(string projectCode, List<string> components)
        {
            var geminiDevelopmentIssueList = filterGeminiIssueList(geminiItemsEngine);

            var developmentLogFile = "DevelopmentLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            foreach (var geminiIssue in geminiDevelopmentIssueList.OrderBy(f => f.Id).ToList())
            {
                try
                {
                    var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                    var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.StoryType, projectCode, components);

                    //Story
                    var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);
                    SetAndSaveReporter(jiraIssue, geminiIssue);
                    var storyFolder = SetAndSaveAlfrescoUrls(jiraIssueInfo, jiraIssue, "");

                    var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != JiraConstants.GroupType && i.Value.Id != currentIssue.Id);

                    //SubTask
                    foreach (var sub in hierarchy)
                    {
                        if (sub.Value.Type == "Task")
                        {
                            var currentSubIssue = geminiItemsEngine.Execute(sub.Value.Id);

                            if (CheckIfValidSubTask(currentSubIssue))
                            {
                                var jiraSubTaskInfo = geminiToJiraMapper.Execute(currentSubIssue, JiraConstants.SubTaskType, projectCode, components);
                                jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                                try
                                {
                                    var subIssue = jiraSaveEngine.Execute(jiraSubTaskInfo);
                                    SetAndSaveReporter(subIssue, currentSubIssue);
                                    SetAndSaveAlfrescoUrls(jiraSubTaskInfo, subIssue, storyFolder);
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText(
                                        JiraConstants.LogDirectory + developmentLogFile,
                                        "[SubT] - " + currentSubIssue.IssueKey + " - " + ex.Message + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(
                        JiraConstants.LogDirectory + developmentLogFile,
                        "[Task] - " + geminiIssue.IssueKey + " - " + ex.Message + Environment.NewLine);
                }
            }
        }

        

        #region Private 
        /// <summary>
        /// The subtask must have the same release and development lines of his father (story)
        /// </summary>
        /// <param name="subTask"></param>
        /// <returns></returns>
        private bool CheckIfValidSubTask(IssueDto subTask)
        {
            var release = subTask.CustomFields.FirstOrDefault(x => x.Name == DevelopmentConstants.DEVELOPMENT_RELEASE_KEY);
            var devLine = subTask.CustomFields.FirstOrDefault(x => x.Name == DevelopmentConstants.DEVELOPMENT_LINE_KEY);

            //Solo i development: i task sono quelli associati ai development trovati
            if (release != null && devLine != null &&
                DevelopmentConstants.DEVELOPMENT_RELEASES.Contains(release.FormattedData) &&
                DevelopmentConstants.DEVELOPMENT_LINES.Contains(devLine.FormattedData))
                return true;
            else
                return false;

        }

        private IEnumerable<IssueDto> filterGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(FilterType.Development));
            var filteredList = Filter.FilterDevIssuesList(geminiIssueList);
            return filteredList;
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter).AccountId;
                jiraIssue.SaveChanges();
            }
        }

        private string SetAndSaveAlfrescoUrls(CreateIssueInfo jiraIssueInfo, Issue jiraIssue, string storyFolder)
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
                folderAlfresco = folderEngine.Execute(AlfrescoConstants.AlfrescoFolder, newFolder, storyFolder);

            if (analysisLink != null)
                jiraIssue.CustomFields.Add("Analysis Document Url", SaveAndUploadToAlfresco(folderAlfresco, analysisLink));

            if (brAnalysisLink != null)
                jiraIssue.CustomFields.Add("BR Analysis Url", SaveAndUploadToAlfresco(folderAlfresco, brAnalysisLink));

            if (changeDocumentLink != null)
                jiraIssue.CustomFields.Add("Change Document Url", SaveAndUploadToAlfresco(folderAlfresco, changeDocumentLink));

            if (testDocumentLink != null)
                jiraIssue.CustomFields.Add("Test Document Url", SaveAndUploadToAlfresco(folderAlfresco, testDocumentLink));

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

        private string SaveAndUploadToAlfresco(IFolder folderAlfresco, LinkItem attachmentLink)
        {
            string url = "";

            if (attachmentLink == null)
                return url;

            if (attachmentLink.FileName == "")
                return attachmentLink.Href;

            attachmentGetter.Save(attachmentLink);

            url = uploadAlfrescoEngine.Execute(folderAlfresco, attachmentLink.FileName);

            if (File.Exists(AlfrescoConstants.AttachmentPath + attachmentLink.FileName))
                File.Delete(AlfrescoConstants.AttachmentPath + attachmentLink.FileName);

            return url;

        }
        #endregion
    }


}
