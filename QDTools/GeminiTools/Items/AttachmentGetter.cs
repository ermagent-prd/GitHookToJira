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
        private readonly IGeminiToolsParameters parameters;
        private readonly List<string> AttachementToExlude = new List<string>
        {
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

        public AttachmentGetter(IGeminiToolsParameters parameters)
        {
            this.parameters = parameters;
        }

        public void Execute(CreateIssueInfo jiraIssue, List<IssueAttachmentDto> attachments)
        {
            
            foreach (var attachment in attachments)
            {
                if (!AttachementToExlude.Contains(attachment.Entity.Name))
                {
                    Save(
                        attachment.Entity.ProjectId,
                        attachment.Entity.IssueId,
                        attachment.Entity.Id,
                        attachment.Entity.Name);

                    jiraIssue.Attachments.Add(attachment.Entity.Name);
                }
            }
        }

        public bool Save(int projectId, int issueId, int attachmentId, string attachmentName)
        {
            try
            {
                var webClient = new WebClient();

                webClient.UseDefaultCredentials = true;

                webClient.DownloadFile(
                    parameters.GEMINI_PATH + projectId + "/item/attachment?issueid=" + issueId + "&fileid=" + attachmentId,
                    parameters.SAVING_PATH + attachmentName);
                
                webClient.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save(LinkItem linkItem)
        {
            if (linkItem == null)
                return false;
            try
            {
                var webClient = new WebClient();

                webClient.UseDefaultCredentials = true;

                webClient.DownloadFile(linkItem.Href,
                    parameters.SAVING_PATH + linkItem.FileName);

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
