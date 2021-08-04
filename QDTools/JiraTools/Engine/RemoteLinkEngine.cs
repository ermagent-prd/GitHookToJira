using System;
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
            if (!IsValid(linkUrl))
                return;

            var cmtTask = addRemoteLink(jiraIssue, linkUrl, title, summary);

            if (cmtTask == null)
                return;

            cmtTask.Wait();

        }



        #endregion

        #region Private methods

        private bool IsValid(string urlAddress)
        {
            Uri uriResult;
            return Uri.TryCreate(urlAddress, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }


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
