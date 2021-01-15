using Atlassian.Jira;
using JiraTools.Parameters;
using System.Collections.Generic;
using System.IO;

namespace JiraTools.Engine
{
    public class AddAttachmentEngine
    {
        public void Execute(Issue issue, List<string> files)
        {
            byte[] byteArray;
            UploadAttachmentInfo uAttachmentInfo;

            foreach (var file in files)
            {
                byteArray = File.ReadAllBytes(Constants.AttachmentPath + file);

                uAttachmentInfo = new UploadAttachmentInfo(file, byteArray);

                issue.AddAttachment(uAttachmentInfo);
            }

            issue.SaveChanges();

            deleteAttachmentsFiles(files);
        }

        private void deleteAttachmentsFiles(List<string> files)
        {
            foreach(var file in files)
            {
                if (File.Exists(Constants.AttachmentPath + file))
                {
                    File.Delete(Constants.AttachmentPath + file);
                }
            }
        }
    }
}
