using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class AddCommentEngine
    {
        #region Private properties
        private readonly ServiceManagerContainer requestFactory;
        #endregion

        public AddCommentEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }



        #region Public methods

        public Comment Execute(string issueKey, string author, string body)
        {
            var comment = new Comment();

            comment.Author = author;
            comment.Body = body;

            var cmtTask = addComment(issueKey, comment);

            if (cmtTask == null)
                return null;

            cmtTask.Wait();

            return cmtTask.Result;
        }


        public Comment Execute(Issue issue, Comment comment)
        {
            var cmtTask = addComment(issue, comment);

            if (cmtTask == null)
                return null;

            cmtTask.Wait();

            return cmtTask.Result;

        }

        public void Execute(Issue issue, string author, string body)
        {
            var comment = new Comment();

            comment.Author = author;
            comment.Body = body;

            Execute(issue, comment);
        }

        public void Execute(Issue issue, List<Comment> commentList)
        {
            if (commentList == null)
                return;

            foreach(var comment in commentList)
                Execute(issue, comment);
        }


        #endregion

        #region Private methods

        private async Task<Comment> addComment(string issueKey, Comment comment)
        {
            var jira = this.requestFactory.Service;

            var issue = await getIssue(jira,issueKey);

            if (issue == null)
                return null;

            return await issue.AddCommentAsync(comment);
        }

        private async Task<Issue> getIssue(Jira jira, string issueKey)
        {
            try
            {
                return await jira.Issues.GetIssueAsync(issueKey);
            }
            catch
            {
                return null;
            }
        }


        private async Task<Comment> addComment(Issue issue, Comment comment)
        {
            return await issue.AddCommentAsync(comment);
        }

        #endregion
    }
}
