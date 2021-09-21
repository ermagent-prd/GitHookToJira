using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeminiIssueProducer.Helpers
{
    internal class AttachmentsHelper
    {
        private const string TEXT_PLAIN = "text/plain";

        private readonly ServiceManager serviceManager;

        public AttachmentsHelper(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public IssueAttachmentDto Add(int issueId, int projectId, string fileName, byte[] allBytes)
        {
            try
            {
                var attachment = new IssueAttachment();

                attachment.Name = fileName;
                attachment.IssueId = issueId;
                attachment.ProjectId = projectId;
                attachment.Content = allBytes;
                attachment.ContentType = TEXT_PLAIN;

                return serviceManager.Item.IssueAttachmentCreate(attachment);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public void Clear(int issueId)
        {
            try
            {
                List<IssueAttachmentDto> attachments =
                    serviceManager.Item.IssueAttachmentsGet(issueId);

                attachments.ForEach(
                    a =>
                    serviceManager.Item.IssueAttachmentDelete(issueId, a.Entity.Id));
            }
            catch (Exception)
            {
            }

        }

    }
}
