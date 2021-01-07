using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Model;
using JiraTools.Service;

namespace JiraTools.Engine
{
    internal class CreateIssueEngine
    {
        private readonly ServiceManagerContainer requestFactory;

        public CreateIssueEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        public Issue Execute(CreateIssueInfo issueFields)
        {
            var task = addIssue(issueFields);

            task.Wait();

            var issue = task.Result;

            //var assigneeTask = assignUser(issueFields, issue);

            //assigneeTask.Wait();

            //var logTask = addWorkLog(issueFields, issue);

            //logTask.Wait();

            return issue;
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


            foreach (var v in fieldsInfo.FixVersions)
                newIssue.FixVersions.Add(v);

            foreach (var c in fieldsInfo.CustomFields)
                newIssue[c.Name] = c.Value;

            foreach (var comp in fieldsInfo.Components)
                newIssue.Components.Add(comp);

            
            newIssue.Assignee = fieldsInfo.Assignee;

            //newIssue["Owner"] = "Pierluigi Nanni";

            return await newIssue.SaveChangesAsync();
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
