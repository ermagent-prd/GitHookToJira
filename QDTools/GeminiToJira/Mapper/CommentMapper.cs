﻿using Atlassian.Jira;
using Atlassian.Jira.Remote;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiToJira.Engine;
using GeminiTools.Items;
using JiraTools.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeminiToJira.Mapper
{
    public class CommentMapper
    {
        private readonly AttachmentGetter attachmentGetter;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;

        public CommentMapper(
            AttachmentGetter attachmentGetter, 
            JiraAccountIdEngine accountEngine,
            ParseCommentEngine parseCommentEngine)
        {
            this.attachmentGetter = attachmentGetter;
            this.accountEngine = accountEngine;
            this.parseCommentEngine = parseCommentEngine;
        }

        public void Execute(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            jiraIssue.CommentList = new List<Comment>();

            for (int i= 0; i < geminiIssue.Comments.Count; i++)
            {
                var comment = geminiIssue.Comments[i];

                var commentPrefix = "Comment_" + i;
                jiraIssue.CommentList.Add(CreateComment(comment.Entity, comment.Attachments, commentPrefix));
                parseCommentEngine.Execute(jiraIssue, comment.Entity.Comment, commentPrefix);

                //Load Comment's attached files
                attachmentGetter.Execute(jiraIssue, comment.Attachments);
            }
        }

        private Comment CreateComment(IssueComment geminiComment, List<IssueAttachmentDto> attachments, string commentPrefix)
        {
            //linka al commento il file allegato nella issue originale
            string commentAttachment = GetAttachmentBody(attachments);

            var author = accountEngine.Execute(geminiComment.Fullname);
            var body = "[~accountId:" + author.AccountId + "]\n" + commentAttachment + parseCommentEngine.Execute(geminiComment.Comment, commentPrefix, null);

            var remoteComment = new RemoteComment();
            remoteComment.author = author.AccountId;
            remoteComment.updateAuthor = author.AccountId;
            remoteComment.updateAuthorUser = author;
            remoteComment.body = body;
            
            var comment = new Comment(remoteComment);
            comment.Author = author.AccountId;
            comment.Body = body;
            
            return comment;

        }

        private string GetAttachmentBody(List<IssueAttachmentDto> attachments)
        {
            StringBuilder commentbody = new StringBuilder();

            if (attachments.Count > 0)
            {

                foreach (var attachment in attachments)
                {
                    commentbody.Append("[^" + attachment.Entity.Name + "]\n\n");
                }
            }

            return commentbody.ToString();
                    

        }
    }
}
