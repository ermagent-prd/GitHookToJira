using System;
using System.Threading;
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

        public CreateIssueEngine(
            ServiceManagerContainer requestFactory,
            AddWorklogEngine worklogEngine,
            AddCommentEngine commentEngine)
        {
            this.requestFactory = requestFactory;
            this.worklogEngine = worklogEngine;
            this.commentEngine = commentEngine;
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

            foreach (var v in fieldsInfo.FixVersions)
                newIssue.FixVersions.Add(v);

            foreach (var c in fieldsInfo.CustomFields)
                newIssue.CustomFields.Add(c.Name, c.Value);

            foreach (var comp in fieldsInfo.Components)
                newIssue.Components.Add(comp);

            newIssue.Assignee = fieldsInfo.Assignee;

            var issue = await newIssue.SaveChangesAsync();

            //TODO to delete
            UpdateAssignee(issue);

            worklogEngine.Execute(issue, fieldsInfo.Logged);

            commentEngine.Execute(issue, fieldsInfo.CommentList);

            return issue;
        }

        //TODO to delete
        private void UpdateAssignee(Issue issue)
        {
            var tmp = requestFactory.Service.Issues.GetIssueAsync(issue.Key.Value, CancellationToken.None);
            tmp.Result.Assignee = "Paolo Luca";
            tmp.Result.SaveChangesAsync().Wait();
        }

        private async Task assignUser(CreateIssueInfo fieldsInfo, Issue newIssue)
        {
            await newIssue.AssignAsync(fieldsInfo.Assignee);
        }


        private async Task<Worklog> addWorkLog(CreateIssueInfo fieldsInfo, Issue newIssue)
        {
            return await newIssue.AddWorklogAsync("1d");
        }

        #endregion

    }
}
