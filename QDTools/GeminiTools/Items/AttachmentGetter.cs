using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Parameters;
using JiraTools.Model;
using System.Collections.Generic;
using System.Net;


namespace GeminiTools.Items
{
    public static class AttachmentGetter
    {
        private static WebClient webClient;

        public static void Execute(CreateIssueInfo jiraIssue, List<IssueAttachmentDto> attachments)
        {
            
            foreach (var attachment in attachments)
            {
                SaveAttachment(
                    attachment.Entity.ProjectId,
                    attachment.Entity.IssueId,
                    attachment.Entity.Id,
                    attachment.Entity.Name);

                jiraIssue.Attachments.Add(attachment.Entity.Name);
            }
        }

        public static bool SaveAttachment(int projectId, int issueId, int attachmentId, string attachmentName)
        {
            try
            {
                CreateWebClient();

                webClient.DownloadFile(
                    Constants.GEMINI_PATH + projectId + "/item/attachment?issueid=" + issueId + "&fileid=" + attachmentId,
                    Constants.SAVING_PATH + attachmentName);

                webClient.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }





        private static void CreateWebClient()
        {
            webClient = new WebClient();
            webClient.UseDefaultCredentials = true;
        }
    }
}
