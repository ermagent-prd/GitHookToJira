using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Model;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class CreateIssueEngine
    {
        private readonly ServiceManagerContainer requestFactory;

        private readonly AddWorklogEngine worklogEngine;
        private readonly AddCommentEngine commentEngine;
        private readonly AddAttachmentEngine attachmentEngineEngine;

        public CreateIssueEngine(
            ServiceManagerContainer requestFactory,
            AddWorklogEngine worklogEngine,
            AddCommentEngine commentEngine,
            AddAttachmentEngine attachmentEngineEngine)
        {
            this.requestFactory = requestFactory;
            this.worklogEngine = worklogEngine;
            this.commentEngine = commentEngine;
            this.attachmentEngineEngine = attachmentEngineEngine;
        }

        public Issue Execute(CreateIssueInfo issueFields)
        {
            var task = addIssue(issueFields);

            task.Wait();

            return task.Result;
        }

        #region Private methods

        private async Task<Issue> addIssue(CreateIssueInfo fieldsInfo)
        {
            var fields = new CreateIssueFields(fieldsInfo.ProjectKey)
            {
                TimeTrackingData = new IssueTimeTrackingData(
                    fieldsInfo.OriginalEstimate,
                    fieldsInfo.RemainingEstimate),
            };

            if (fieldsInfo.Type.Name == "Sottotask")
                fields.ParentIssueKey = fieldsInfo.ParentIssueKey;
            
            var newIssue = new Issue(this.requestFactory.Service, fields);

            newIssue.Type = fieldsInfo.Type;
            newIssue.Priority = fieldsInfo.Priority;
            newIssue.Summary = fieldsInfo.Summary;
            newIssue.Description = fieldsInfo.Description;
            newIssue.DueDate = fieldsInfo.DueDate;
            //TODO
            //newIssue.Reporter = fieldsInfo.Reporter;

            foreach (var v in fieldsInfo.FixVersions)
                newIssue.FixVersions.Add(v);

            foreach (var c in fieldsInfo.CustomFields)
                newIssue.CustomFields.Add(c.Name, c.Value);

            foreach (var comp in fieldsInfo.Components)
                newIssue.Components.Add(comp);

            newIssue.Assignee = fieldsInfo.Assignee;

            var issue = await newIssue.SaveChangesAsync();
            
            worklogEngine.Execute(issue, fieldsInfo.Logged);

            commentEngine.Execute(issue, fieldsInfo.CommentList);

            attachmentEngineEngine.Execute(issue, fieldsInfo.Attachments);

            return issue;
        }


        #endregion

    }
}
