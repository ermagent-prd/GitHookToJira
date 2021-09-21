using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiIssueProducer.Parameters;
using System;

namespace GeminiIssueProducer.Helpers
{
    internal class UpdateCommentHelper
    {
        private readonly ServiceManager serviceManager;

        public UpdateCommentHelper(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public IssueCommentDto Execute(int issueId, IssueParams parameters)
        {
            IssueComment comment =
                GetComment(
                    issueId,
                    parameters.FixedParams.ProjectIdValue,
                    parameters.FreeParams.Comment,
                    parameters.FixedParams.UserId);

            try
            {
                return
                    serviceManager.Item.IssueCommentCreate(comment);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IssueComment GetComment(
            int issueId,
            int projecyId,
            string text,
            int userId)
        {
            var comment =
                new IssueComment();

            comment.Comment = text;
            comment.ProjectId = projecyId;
            comment.IssueId = issueId;
            comment.UserId = userId;

            return comment;
        }
    }
}
