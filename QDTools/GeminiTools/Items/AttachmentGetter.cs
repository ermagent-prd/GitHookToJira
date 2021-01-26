using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Parameters;
using JiraTools.Model;
using System.Collections.Generic;
using System.Net;


namespace GeminiTools.Items
{
    public class AttachmentGetter
    {
        private readonly IGeminiToolsParameters parameters;

        public AttachmentGetter(IGeminiToolsParameters parameters)
        {
            this.parameters = parameters;
        }

        public void Execute(CreateIssueInfo jiraIssue, List<IssueAttachmentDto> attachments)
        {
            
            foreach (var attachment in attachments)
            {
                Save(
                    attachment.Entity.ProjectId,
                    attachment.Entity.IssueId,
                    attachment.Entity.Id,
                    attachment.Entity.Name);

                jiraIssue.Attachments.Add(attachment.Entity.Name);
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

    }
}
