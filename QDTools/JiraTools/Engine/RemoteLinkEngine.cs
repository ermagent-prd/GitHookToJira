using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class RemoteLinkEngine
    {
        #region Private properties
        private readonly ServiceManagerContainer requestFactory;
        #endregion

        public RemoteLinkEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }



        #region Public methods

        public void Execute(Issue jiraIssue,
            string linkUrl,
            string title,
            string summary = null)
        {
            var cmtTask = addRemoteLink(jiraIssue, linkUrl, title, summary);

            if (cmtTask == null)
                return;

            cmtTask.Wait();

        }



        #endregion

        #region Private methods

        private async Task addRemoteLink(
            Issue jiraIssue,
            string linkUrl,
            string title,
            string summary = null)
        {
            await jiraIssue.AddRemoteLinkAsync(linkUrl, title, summary);
        }


        private async Task<Comment> addComment(Issue issue, Comment comment)
        {
            return await issue.AddCommentAsync(comment);
        }

        #endregion
    }
}
