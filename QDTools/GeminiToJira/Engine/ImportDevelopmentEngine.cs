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
                //try
                //{
                    var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                    var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.StoryTpe, projectCode, components);

                    var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);
                    SetAndSaveReporter(jiraIssue, geminiIssue);
                    SetAndSaveAlfrescoUrl(jiraIssueInfo, jiraIssue);

                    var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != JiraConstants.GroupType && i.Value.Id != currentIssue.Id);

                    foreach (var sub in hierarchy)
                    {
                        if (sub.Value.Type == "Task")
                        {
                            var jiraSubTaskInfo = geminiToJiraMapper.Execute(sub.Value, JiraConstants.SubTaskType, projectCode, components);
                            jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                            try
                            {
                                var subIssue = jiraSaveEngine.Execute(jiraSubTaskInfo);
                                SetAndSaveReporter(subIssue, sub.Value);
                                SetAndSaveAlfrescoUrl(jiraSubTaskInfo, subIssue);
                            }
                            catch (Exception ex)
                            {
                                File.AppendAllText(
                                    JiraConstants.LogDirectory + developmentLogFile,
                                    "[SubT] - " + sub.Value.IssueKey + " - " + ex.Message + Environment.NewLine);
                            }
                        }
                    }
                //}
                //catch (Exception ex)
                //{
                //    File.AppendAllText(
                //        JiraConstants.LogDirectory + developmentLogFile,
                //        "[Task] - " + geminiIssue.IssueKey + " - " + ex.Message + Environment.NewLine);
                //}
            }
        }

        

        #region Private 
        private IEnumerable<IssueDto> filterGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(FilterType.Development));
            Filter.FilterDevIssuesList(geminiIssueList);
            return geminiIssueList;
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter).AccountId;
                jiraIssue.SaveChanges();
            }
        }

        private void SetAndSaveAlfrescoUrl(CreateIssueInfo jiraIssueInfo, Issue jiraIssue)
        {
            if (!HasToCreateFolder(jiraIssueInfo))
                return;

            var newFolder = jiraIssue.Key.Value;
            var folderAlfresco = folderEngine.Execute(AlfrescoConstants.AlfrescoFolder, newFolder);
            
            if (jiraIssueInfo.AnalysisUrl != null && jiraIssueInfo.AnalysisUrl != "")
                jiraIssue.CustomFields.Add("Analysis Document Url", SaveAndUploadToAlfresco(folderAlfresco, jiraIssueInfo.AnalysisUrl));

            if (jiraIssueInfo.BrAnalysisUrl != null && jiraIssueInfo.BrAnalysisUrl != "")
                jiraIssue.CustomFields.Add("BR Analysis Url", SaveAndUploadToAlfresco(folderAlfresco, jiraIssueInfo.BrAnalysisUrl));

            if (jiraIssueInfo.ChangeDocumentUrl != null && jiraIssueInfo.ChangeDocumentUrl != "")
                jiraIssue.CustomFields.Add("Change Document Url", SaveAndUploadToAlfresco(folderAlfresco, jiraIssueInfo.ChangeDocumentUrl));

            if (jiraIssueInfo.TestDocumentUrl != null && jiraIssueInfo.TestDocumentUrl != "")
                jiraIssue.CustomFields.Add("Test Document Url", SaveAndUploadToAlfresco(folderAlfresco, jiraIssueInfo.TestDocumentUrl));

            jiraIssue.SaveChanges();
        }

        private bool HasToCreateFolder(CreateIssueInfo jiraIssueInfo)
        {
            if ((jiraIssueInfo.AnalysisUrl != null && jiraIssueInfo.AnalysisUrl != "") ||
                (jiraIssueInfo.BrAnalysisUrl != null && jiraIssueInfo.BrAnalysisUrl != "") ||
                (jiraIssueInfo.ChangeDocumentUrl != null && jiraIssueInfo.ChangeDocumentUrl != "") ||
                (jiraIssueInfo.TestDocumentUrl != null && jiraIssueInfo.TestDocumentUrl != ""))
                return true;
            else
                return false;

        }

        private string SaveAndUploadToAlfresco(IFolder folderAlfresco, string attachmentUrl)
        {
            string url = "";
            var link = linkItemEngine.Execute(attachmentUrl);

            attachmentGetter.Save(link);

            url = uploadAlfrescoEngine.Execute(folderAlfresco, link.FileName);

            if (File.Exists(AlfrescoConstants.AttachmentPath + link.FileName))
                File.Delete(AlfrescoConstants.AttachmentPath + link.FileName);

            return url;
        }
        #endregion
    }


}
