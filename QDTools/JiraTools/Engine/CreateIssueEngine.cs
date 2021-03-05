using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Model;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class CreateIssueEngine
    {
        private readonly Dictionary<string, string> RESOLUTION_DICTIONARY = new Dictionary<string, string>()
        {
            { "Done",           "10000" },
            { "Won't Do",       "10001" },
            { "Duplicate",      "10002" },
            {"Cannot Reproduce","10003" }
        };

        //TODO to map
        private readonly Dictionary<string, string> RESOLUTION_MAPPING = new Dictionary<string, string>()
        {
            { "Completed",   "Done" },
            { "Unresolved",   "Won't Do" },
            { "Duplicate",  "Duplicate" },        };


        private readonly string SubTaskType = "10003";
        private readonly string UatType = "10014";

        private readonly ServiceManagerContainer requestFactory;

        private readonly AddWorklogEngine worklogEngine;
        private readonly AddCommentEngine commentEngine;
        private readonly AddAttachmentEngine attachmentEngine;
        

        public CreateIssueEngine(
            ServiceManagerContainer requestFactory,
            AddWorklogEngine worklogEngine,
            AddCommentEngine commentEngine,
            AddAttachmentEngine attachmentEngineEngine)
        {
            this.requestFactory = requestFactory;
            this.worklogEngine = worklogEngine;
            this.commentEngine = commentEngine;
            this.attachmentEngine = attachmentEngineEngine;
        }

        public Issue Execute(CreateIssueInfo issueFields, string attachmentPath)
        {
            var task = addIssue(issueFields, attachmentPath);

            task.Wait();

            return task.Result;
        }

        #region Private methods

        private async Task<Issue> addIssue(CreateIssueInfo fieldsInfo, string attachmentPath)
        {
            IssueTimeTrackingData timeTrackingData = null;

            if (fieldsInfo.Type.Id != UatType)
                timeTrackingData = new IssueTimeTrackingData(
                    fieldsInfo.OriginalEstimate,
                    fieldsInfo.RemainingEstimate);

            var fields = new CreateIssueFields(fieldsInfo.ProjectKey)
            {
                    TimeTrackingData = timeTrackingData
            };

            if (fieldsInfo.Type.Id == SubTaskType)
                fields.ParentIssueKey = fieldsInfo.ParentIssueKey;

            var newIssue = new Issue(this.requestFactory.Service, fields);
            
            newIssue.Summary = fieldsInfo.Summary;
            newIssue.Description = fieldsInfo.Description;
            newIssue.Type = fieldsInfo.Type;
            
            if (fieldsInfo.Priority != null)
                newIssue.Priority = fieldsInfo.Priority;

            if (fieldsInfo.DueDate.HasValue)
                newIssue.DueDate = fieldsInfo.DueDate.Value;

            if (fieldsInfo.Resolution != null && fieldsInfo.Resolution != "")
            {
                string mappedResolution;
                if (RESOLUTION_MAPPING.TryGetValue(fieldsInfo.Resolution, out mappedResolution))
                {
                    string jiraResolutionId;
                    if (RESOLUTION_DICTIONARY.TryGetValue(mappedResolution, out jiraResolutionId))
                        newIssue.Resolution = new IssueResolution(jiraResolutionId, mappedResolution);
                }
            }

            SetFixVersions(fieldsInfo, newIssue);

            SetAffectVersions(fieldsInfo, newIssue);

            foreach (var c in fieldsInfo.CustomFields)
                newIssue.CustomFields.Add(c.Name, c.Value);

            foreach (var comp in fieldsInfo.Components)
                newIssue.Components.Add(comp);

            newIssue.Assignee = fieldsInfo.Assignee;

            var issue = await newIssue.SaveChangesAsync();

            worklogEngine.Execute(issue, fieldsInfo.Logged);

            commentEngine.Execute(issue, fieldsInfo.CommentList);

            attachmentEngine.Execute(issue, fieldsInfo.Attachments, attachmentPath);

            return issue;
        }

        private static void SetAffectVersions(CreateIssueInfo fieldsInfo, Issue newIssue)
        {
            if (fieldsInfo.AffectVersions == null)
                return;

            foreach (var v in fieldsInfo.AffectVersions)
                try
                {
                    newIssue.AffectsVersions.Add(v);
                }
                catch
                { }
        }

        private static void SetFixVersions(CreateIssueInfo fieldsInfo, Issue newIssue)
        {
            if (fieldsInfo.FixVersions == null)
                return;

            foreach (var v in fieldsInfo.FixVersions)
                try
                {
                    newIssue.FixVersions.Add(v);
                }
                catch
                { }
        }


        #endregion

    }
}
