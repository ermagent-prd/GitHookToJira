using System.IO;
using AlfrescoTools.Engine;
using Atlassian.Jira;
using DotCMIS.Client;
using GeminiToJira.Parameters.Import;
using GeminiTools.Engine;
using GeminiTools.Items;
using JiraTools.Model;

namespace GeminiToJira.Engine.Common.Alfresco
{
    public class AlfrescoUrlsEngine
    {
        #region Private properties

        private readonly JiraRemoteLinkerEngine remoteLinkEngine;

        private readonly FolderCreateEngine folderEngine;

        private readonly URLChecker urlChecker;

        private readonly LinkItemEngine linkItemEngine;

        private readonly UploadDocumentEngine uploadAlfrescoEngine;

        private readonly AttachmentGetter attachmentGetter;



        #endregion

        #region Constructor

        public AlfrescoUrlsEngine(
            JiraRemoteLinkerEngine remoteLinkEngine,
            FolderCreateEngine folderEngine,
            URLChecker urlChecker,
            LinkItemEngine linkItemEngine,
            UploadDocumentEngine uploadAlfrescoEngine,
            AttachmentGetter attachmentGetter)
        {
            this.remoteLinkEngine = remoteLinkEngine;
            this.folderEngine = folderEngine;
            this.urlChecker = urlChecker;
            this.linkItemEngine = linkItemEngine;
            this.uploadAlfrescoEngine = uploadAlfrescoEngine;
            this.attachmentGetter = attachmentGetter;
        }


        #endregion

        #region Public methods

        public string Execute(
            CreateIssueInfo jiraIssueInfo,
            Issue jiraIssue,
            string rooStoryFolder,
            GeminiToJiraParameters configurationSetup)
        {
            //Set Analysis links
            if (!string.IsNullOrWhiteSpace(jiraIssueInfo.AnalysisUrl) && 
                (jiraIssue.Type.Id == configurationSetup.Jira.StoryTypeCode ||
                jiraIssue.Type.Id == configurationSetup.Jira.EpicTypeCode))
            {
                this.remoteLinkEngine.Execute(
                    jiraIssue,
                    jiraIssueInfo.AnalysisUrl,
                    "Analysis Documentation");
            }

            LinkItem brAnalysisLink = null;
            LinkItem changeDocumentLink = null;
            LinkItem testDocumentLink = null;
            LinkItem newFeatureDocumentUrl = null;

            GetLinks(jiraIssueInfo, ref brAnalysisLink, ref changeDocumentLink, ref testDocumentLink, ref newFeatureDocumentUrl);

            //if all links are anull no url to save
            if (brAnalysisLink == null &&
                changeDocumentLink == null &&
                testDocumentLink == null &&
                newFeatureDocumentUrl == null)
                return "";

            var newFolder = jiraIssue.Summary;
            IFolder folderAlfresco = null;


            //new folder is needed only if there is , at least, one File to save (url don't need folders)
            if ((brAnalysisLink != null && brAnalysisLink.FileName != "") ||
                (changeDocumentLink != null && changeDocumentLink.FileName != "") ||
                (testDocumentLink != null && testDocumentLink.FileName != "") ||
                (newFeatureDocumentUrl != null && newFeatureDocumentUrl.FileName != "")
                )
                folderAlfresco = this.folderEngine.Execute(configurationSetup.Alfresco.RootFolder, newFolder, rooStoryFolder);

            manageDocLink(
                jiraIssue,
                brAnalysisLink,
                folderAlfresco,
                configurationSetup,
                "Business requirements");

            manageDocLink(
                jiraIssue,
                changeDocumentLink,
                folderAlfresco,
                configurationSetup,
                "Changes document");

            manageDocLink(
                jiraIssue,
                testDocumentLink,
                folderAlfresco,
                configurationSetup,
                "Test document");

            manageDocLink(
                jiraIssue,
                newFeatureDocumentUrl,
                folderAlfresco,
                configurationSetup,
                "New Feature document");

            //TODOPL save alfresco folder in csustomField if and only if folderAlfresco != null
            if (folderAlfresco != null && 
                (jiraIssue.Type.Id == configurationSetup.Jira.StoryTypeCode ||
                jiraIssue.Type.Id == configurationSetup.Jira.EpicTypeCode))
            {
                var docUrl = configurationSetup.Alfresco.FolderLinkPrefix + folderAlfresco.Id;

                jiraIssue.CustomFields.Add(
                    "Documentation Url",
                    docUrl);

                this.remoteLinkEngine.Execute(
                    jiraIssue,
                    docUrl,
                    "Documentation Url");

                jiraIssue.SaveChanges();
            }

            return newFolder;
        }


        #endregion

        #region Private methods

        private void manageDocLink(
            Issue jiraIssue,
            LinkItem docLink,
            IFolder folderAlfresco,
            GeminiToJiraParameters configurationSetup,
            string docTitle)
        {
            if (docLink == null)
                return;

            var docUrl = SaveAndUploadToAlfresco(folderAlfresco, docLink, configurationSetup);

            this.remoteLinkEngine.Execute(jiraIssue, docUrl, docTitle);
        }

        private void GetLinks(CreateIssueInfo jiraIssueInfo,
            ref LinkItem brAnalysisLink,
            ref LinkItem changeDocumentLink,
            ref LinkItem testDocumentLink,
            ref LinkItem newFeatureDocumentUrl)
        {
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
                    Href = this.urlChecker.Execute(attachmentUrl),
                    FileName = ""
                };
            else
            {
                return this.linkItemEngine.Execute(attachmentUrl);
            }
        }

        private string SaveAndUploadToAlfresco(IFolder folderAlfresco, LinkItem attachmentLink, GeminiToJiraParameters configurationSetup)
        {
            string url = "";

            if (attachmentLink == null)
                return url;

            if (attachmentLink.FileName == "")
                return attachmentLink.Href;

            this.attachmentGetter.Save(attachmentLink, configurationSetup.AttachmentDownloadedPath);

            url = this.uploadAlfrescoEngine.Execute(folderAlfresco, attachmentLink.FileName, configurationSetup.AttachmentDownloadedPath);

            if (File.Exists(configurationSetup.AttachmentDownloadedPath + attachmentLink.FileName))
                File.Delete(configurationSetup.AttachmentDownloadedPath + attachmentLink.FileName);

            return url;

        }



        #endregion
    }
}
