using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Parameters;
using JiraTools.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace GeminiTools.Items
{
    public class AttachmentGetter
    {
        
        /// <summary>
        /// Image used only for email signature
        /// </summary>
        private readonly List<string> AttachementToExlude = new List<string>
        {
            "image001.png",
            "image002.png",
            "image003.png",
            "image004.png",
            "image005.png",
            "image006.png",
            "image007.png",
            "image008.png",
            "image009.png",
            "image010.png",
        };

        public AttachmentGetter()
        {
           
        }

        public void Execute(CreateIssueInfo jiraIssue, List<IssueAttachmentDto> attachments, string projectUrl, string attachmentPath)
        {
            
            foreach (var attachment in attachments)
            {
                if (!AttachementToExlude.Contains(attachment.Entity.Name))
                {
                    FileDownload(
                        attachment.Entity.ProjectId,
                        attachment.Entity.IssueId,
                        attachment.Entity.Id,
                        attachment.Entity.Name,
                        projectUrl,
                        attachmentPath);

                    jiraIssue.Attachments.Add(attachment.Entity.Name);
                }
            }
        }

        public bool FileDownload(int projectId, int issueId, int attachmentId, string attachmentName, string projectUrl, string attachmentPath)
        {
            try
            {
                var webClient = new WebClient();

                webClient.UseDefaultCredentials = true;

                webClient.DownloadFile(
                    projectUrl + projectId + "/item/attachment?issueid=" + issueId + "&fileid=" + attachmentId,
                    attachmentPath + attachmentName);
                
                webClient.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save(LinkItem linkItem, string attachmentDownloadedPath)
        {
            if (linkItem == null)
                return false;
            try
            {
                var webClient = new WebClient();

                webClient.UseDefaultCredentials = true;

                webClient.DownloadFile(linkItem.Href,
                    attachmentDownloadedPath + linkItem.FileName);

                webClient.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
