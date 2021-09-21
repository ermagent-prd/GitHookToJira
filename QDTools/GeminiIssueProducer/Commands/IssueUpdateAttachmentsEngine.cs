using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using GeminiIssueProducer.Helpers;
using GeminiIssueProducer.Parameters;
using System;
using System.IO;

namespace GeminiIssueProducer.Commands
{
    internal class IssueUpdateAttachmentsEngine
    {
        private readonly AttachmentsHelper attachmentsHelper;

        public IssueUpdateAttachmentsEngine(ServiceManager serviceManager)
        {
            attachmentsHelper =
                new AttachmentsHelper(serviceManager);
        }

        public IGeminiCommandOutput Execute(IssueParams parameters, int issueId, bool clear)
        {
            foreach (string filePath in parameters.FreeParams.Attachments)
            {
                string fileName = Path.GetFileName(filePath);
                byte[] fileContent =
                        GetFileContent(filePath);

                if (fileContent == null)
                    return new SimpleCommandOutput(false, GeminiConstants.ERR_CANNOT_READ_FILE);

                if (clear)
                    attachmentsHelper.Clear(issueId);

                IssueAttachmentDto attachmentObj =
                    attachmentsHelper.Add(
                        issueId,
                        parameters.FixedParams.ProjectIdValue,
                        fileName,
                        fileContent);

                if (attachmentObj == null)
                    return new SimpleCommandOutput(false, GeminiConstants.ERR_CANNOT_UPDATE_ATTACHMENTS);
            }

            return new SimpleCommandOutput(true, GeminiConstants.OK);
        }

        private Byte[] GetFileContent(string fullPath)
        {
            try
            {
                return
                    File.ReadAllBytes(fullPath);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
