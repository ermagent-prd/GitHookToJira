using Atlassian.Jira;
using JiraTools.Parameters;
using System.Collections.Generic;
using System.IO;

namespace JiraTools.Engine
{
    public class AddAttachmentEngine
    {

        public AddAttachmentEngine()
        {
        }

        public void Execute(Issue issue, List<string> files, string attachmentPath)
        {
            if (files == null)
                return;

            byte[] byteArray;
            UploadAttachmentInfo uAttachmentInfo;

            foreach (var file in files)
            {
                try
                {
                    byteArray = File.ReadAllBytes(attachmentPath + file);

                    uAttachmentInfo = new UploadAttachmentInfo(file, byteArray);

                    issue.AddAttachment(uAttachmentInfo);
                }
                catch
                {
                    //for file not found
                }
            }

            issue.SaveChanges();

            deleteAttachmentsFiles(files, attachmentPath);
        }

        private void deleteAttachmentsFiles(List<string> files, string attachmentPath)
        {
            foreach (var file in files)
            {
                if (File.Exists(attachmentPath + file))
                {
                    File.Delete(attachmentPath + file);
                }
            }
        }
    }
}
