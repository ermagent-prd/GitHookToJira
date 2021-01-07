using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraTools.Engine
{
    internal class AddCommentEngine
    {
        #region Public methods

        public Comment Execute(Issue issue, Comment comment)
        {
            var cmtTask = addComment(issue, comment);

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


        #endregion

        #region Private methods

        private async Task<Comment> addComment(Issue issue, Comment comment)
        {

            return await issue.AddCommentAsync(comment);
        }

        #endregion
    }
}
