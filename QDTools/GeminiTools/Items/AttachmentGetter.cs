using GeminiTools.Parameters;
using System.Net;


namespace GeminiTools.Items
{
    public static class AttachmentGetter
    {
        private static WebClient webClient;


        public static bool Save(int projectId, int issueId, int attachmentId, string attachmentName)
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
