using Atlassian.Jira;
using JiraTools.Engine;
using JiraTools.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraToJira.Engine
{
    public class CloneIssueEngine
    {
        private readonly ServiceManagerContainer requestFactory;
        private readonly AddWorklogEngine worklogEngine;
        private readonly AddCommentEngine commentEngine;
        private readonly AddAttachmentEngine attachmentEngine;


        public CloneIssueEngine(
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

        internal Issue Execute(Issue issue, string destProject, Issue relatedDev)
        {
            var task = cloneIssue(issue, destProject, relatedDev);

            task.Wait();

            return task.Result;
        }


        private async Task<Issue> cloneIssue(Issue issue, string destProject, Issue relatedDev)
        {

            IssueTimeTrackingData timeTrackingData = null;

            if (issue.Type.Id == "10001")
                timeTrackingData = issue.TimeTrackingData;

            var fields = new CreateIssueFields(destProject)
            {
                TimeTrackingData = timeTrackingData
            };


            //da gestire
            if (relatedDev != null)
                fields.ParentIssueKey = relatedDev.Key.Value;

            var newIssue = new Issue(this.requestFactory.Service, fields);

            newIssue.Summary = "CLONED - " + issue.Summary;
            newIssue.Description = issue.Description;
            newIssue.Type = issue.Type;

            newIssue.Priority = issue.Priority;

            newIssue.DueDate = issue.DueDate;
            newIssue.Resolution = issue.Resolution;

            //foreach (var v in issue.FixVersions)
            //    newIssue.FixVersions.Add(v);
            //
            //foreach (var a in issue.AffectsVersions)
            //    newIssue.AffectsVersions.Add(a);

            //foreach (var c in issue.CustomFields)
            //{
            //    if(c.Id != "customfield_10018" &&   //parent_link
            //        c.Id != "customfield_10019" &&  //rank
            //        c.Id != "customfield_10000" &&  //development
            //        c.Id != "customfield_10011" &&  //epic name
            //        c.Id != "customfield_10014")    //epic_link
            //        newIssue.CustomFields.Add(c.Name, c.Values);
            //}

            if (issue.Type.Id == "10000")
                newIssue.CustomFields.Add("Epic Name", issue.CustomFields.FirstOrDefault(x => x.Name == "Epic Name").Values[0]);

            //foreach (var comp in issue.Components)
            //    newIssue.Components.Add(comp);

            newIssue.Assignee = issue.Assignee;

            var clonedIssue = await newIssue.SaveChangesAsync();

            var workLogs = await issue.GetWorklogsAsync();
            foreach(var w in workLogs)
                worklogEngine.Execute(clonedIssue, w, WorklogStrategy.AutoAdjustRemainingEstimate);

            var comments = await issue.GetCommentsAsync();
            foreach (var c in comments)
                commentEngine.Execute(clonedIssue, c);

            var attachments = await issue.GetAttachmentsAsync();

            foreach (var attach in attachments)
            {
                var byteArray = attach.DownloadData();

                var uAttachmentInfo = new UploadAttachmentInfo(attach.FileName, byteArray);

                clonedIssue.AddAttachment(uAttachmentInfo);
            }

            var watchers = await issue.GetWatchersAsync();
            foreach (var w in watchers)
                await clonedIssue.AddWatcherAsync(w.AccountId);

            clonedIssue.Reporter = issue.Reporter;
            clonedIssue.SaveChanges();

            return clonedIssue;
        }
    }
}
