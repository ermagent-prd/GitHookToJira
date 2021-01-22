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
                    GetAuthor(comment.Entity.Fullname, userDictionary) +
                    ParseCommentEngine.Execute(comment.Entity.Comment);
                jiraIssue.CommentList.Add(jiraComment);

                //Load Comment attachments
                AttachmentGetter.Execute(jiraIssue, comment.Attachments);
            }
        }

        private static string GetAuthor(string fullname, Dictionary<string, JiraUser> userDictionary)
        {
            JiraUser author = null;
            userDictionary.TryGetValue(fullname, out author);

            return author != null ?
                "[~accountId:" + author.AccountId + "]: " :
                fullname + ": ";
        }
    }
}
