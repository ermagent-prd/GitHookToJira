using System;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Model;
using JiraTools.Service;

namespace JiraTools.Engine
{
    internal class CreateIssueEngine
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

            var newIssue = new Issue(this.requestFactory.Service, fields);

            newIssue.Type = fieldsInfo.Type;
            newIssue.Priority = fieldsInfo.Priority;
            newIssue.Summary = fieldsInfo.Summary;
            newIssue.Description = fieldsInfo.Description;
            newIssue.DueDate = fieldsInfo.DueDate;

            newIssue.Assignee = "lucap";

            foreach (var v in fieldsInfo.FixVersions)
                newIssue.FixVersions.Add(v);

            foreach (var c in fieldsInfo.CustomFields)
                newIssue.CustomFields.Add(c.Name, c.Value);

            foreach (var comp in fieldsInfo.Components)
                newIssue.Components.Add(comp);


            //newIssue.Assignee = fieldsInfo.Assignee;

            var issue = await newIssue.SaveChangesAsync();

            //await issue.AssignAsync("accountId=\""+ fieldsInfo.Assignee +"\"");

            //issue.AddWorklogAsync()

            worklogEngine.Execute(issue, "Paolo Luca", "1d", DateTime.Now, "Work Log message");

            commentEngine.Execute(issue, "Paolo Luca", "body comment");

            //issue.Assignee = "c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            //
            //issue.AssignAsync("c13ce356-ec00-4ffd-b615-a45a86aa99e2").Wait();
            //
            //issue.SetPropertyAsync("Assignee", "c13ce356-ec00-4ffd-b615-a45a86aa99e2").Wait();

            return issue;
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
