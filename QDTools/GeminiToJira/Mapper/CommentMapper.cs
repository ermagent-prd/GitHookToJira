using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiTools.Items;
using JiraTools.Model;
using System.Collections.Generic;


namespace GeminiToJira.Mapper
{
    public static class CommentMapper
    {
        public static void Execute(CreateIssueInfo jiraIssue, IssueDto geminiIssue, Dictionary<string, JiraUser> userDictionary)
        {
            jiraIssue.CommentList = new List<Comment>();

            foreach (var comment in geminiIssue.Comments)
            {
                var jiraComment = new Comment();
                jiraComment.Author = comment.Entity.Fullname;
                
                jiraComment.Body = 
                    "[~accountId:70121:67b933a3-5693-47d2-82c0-3f997f279387] wrote Body Comment" + //TODO dinamico con il dizionario utenti [trygetvalue from comment.Entity.Fullname]
                    comment.Entity.Fullname + " : " + 
                    ParseCommentEngine.Execute(comment.Entity.Comment);
                jiraIssue.CommentList.Add(jiraComment);

                //Load Comment attachments
                AttachmentGetter.Execute(jiraIssue, comment.Attachments);
            }
        }

    }
}
