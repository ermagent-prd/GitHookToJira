using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiTools.Items;
using JiraTools.Model;
using System.Collections.Generic;


namespace GeminiToJira.Mapper
{
    public class CommentMapper
    {
        private readonly AttachmentGetter attachmentGetter;
        private readonly JiraAccountIdEngine accountEngine;

        public CommentMapper(AttachmentGetter attachmentGetter, JiraAccountIdEngine accountEngine)
        {
            this.attachmentGetter = attachmentGetter;
            this.accountEngine = accountEngine;
        }

        public void Execute(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            jiraIssue.CommentList = new List<Comment>();

            foreach (var comment in geminiIssue.Comments)
            {
                var jiraComment = new Comment();
                jiraComment.Author = comment.Entity.Fullname;

                jiraComment.Body =
                    GetAuthor(comment.Entity.Fullname) +
                    ParseCommentEngine.Execute(comment.Entity.Comment);
                jiraIssue.CommentList.Add(jiraComment);

                //Load Comment attachments
                attachmentGetter.Execute(jiraIssue, comment.Attachments);
            }
        }

        private string GetAuthor(string fullname)
        {
            return "[~accountId:" + accountEngine.Execute(fullname) + "]: ";
        }
    }
}
