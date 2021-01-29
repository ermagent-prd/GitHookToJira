﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Model;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class CreateIssueEngine
    {
        private Dictionary<string, string> RESOLUTION_DICTIONARY = new Dictionary<string, string>()
        {
            { "Done",           "10000" },
            { "Won't Do",       "10001" },
            { "Duplicate",      "10002" },
            {"Cannot Reproduce","10003" }
        };

        //TODO da decidere
        private Dictionary<string, string> RESOLUTION_MAPPING = new Dictionary<string, string>()
        {
            { "Completed",   "Done" },
            { "Unresolved",   "Won't Do" },
            { "Duplicate",  "Duplicate" },        };


        private readonly string SubTaskType = "10003";

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
                    fieldsInfo.RemainingEstimate)
            };

            if (fieldsInfo.Type.Id == SubTaskType)
                fields.ParentIssueKey = fieldsInfo.ParentIssueKey;
            
            var newIssue = new Issue(this.requestFactory.Service, fields);

            newIssue.Summary = fieldsInfo.Summary;
            newIssue.Description = fieldsInfo.Description;
            newIssue.Type = fieldsInfo.Type;

            if(fieldsInfo.Priority != null)
                newIssue.Priority = fieldsInfo.Priority;
            
            if(fieldsInfo.DueDate.HasValue)
                newIssue.DueDate = fieldsInfo.DueDate.Value;
                        
            
            if (fieldsInfo.Resolution != null && fieldsInfo.Resolution != "")
            {
                string mappedResolution;
                if (RESOLUTION_MAPPING.TryGetValue(fieldsInfo.Resolution, out mappedResolution))
                {
                    string jiraResolutionId;
                    if(RESOLUTION_DICTIONARY.TryGetValue(mappedResolution, out jiraResolutionId))
                        newIssue.Resolution = new IssueResolution(jiraResolutionId, mappedResolution);
                }
            }

            foreach (var v in fieldsInfo.FixVersions)
                newIssue.FixVersions.Add(v);

            foreach (var v in fieldsInfo.AffectVersions)
                newIssue.AffectsVersions.Add(v);

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
