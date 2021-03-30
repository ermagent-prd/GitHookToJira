using Atlassian.Jira;
using JiraTools.Engine;
using JiraTools.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JiraToJira.Container.Jira;
using System.Threading.Tasks;
using GeminiToJira.Engine;

namespace JiraToJira.Engine
{
    public class CloneIssueEngine
    {
        private readonly ServiceManagerContainer requestFactory;
        private readonly AddWorklogEngine worklogEngine;
        private readonly AddCommentEngine commentEngine;
        private readonly JiraAccountIdEngine accountEngine;

        private readonly Dictionary<string, string> issueTypeMapping = new Dictionary<string, string>();
        private readonly Dictionary<string, string> customFieldsMapping = new Dictionary<string, string>();
        
        public CloneIssueEngine(
            ServiceManagerContainer requestFactory,
            AddWorklogEngine worklogEngine,
            AddCommentEngine commentEngine,
            AddAttachmentEngine attachmentEngineEngine)
        {
            this.requestFactory = requestFactory;
            this.worklogEngine = worklogEngine;
            this.commentEngine = commentEngine;
            this.accountEngine = UserAccountEngine();

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

            CloneIssueType(issue, newIssue);

            newIssue.Priority = issue.Priority;

            newIssue.DueDate = issue.DueDate;
            //newIssue.Resolution = issue.Resolution;

            //CloneVersions(issue, newIssue);
            //CloneComponents(issue, newIssue);

            CloneCustomFields(issue, newIssue);

            if (issue.Type.Id == "10000")
                newIssue.CustomFields.Add("Epic Name", issue.CustomFields.FirstOrDefault(x => x.Name == "Epic Name").Values[0]);

            newIssue.Assignee = SetUser(issue.AssigneeUser.Username);

            var clonedIssue = await newIssue.SaveChangesAsync();

            await CloneWorkLogs(issue, clonedIssue);

            await CloneComments(issue, clonedIssue);

            await CloneAttachments(issue, clonedIssue);

            await CloneWatchers(issue, clonedIssue);

            clonedIssue.Reporter = SetUser(issue.ReporterUser.Username);
            clonedIssue.SaveChanges();

            return clonedIssue;
        }

        private JiraAccountIdEngine UserAccountEngine()
        {
            var serviceManagerContainer = new ServiceManagerContainer(new JiraToJiraParamContainer());
            var userListGetter = new UserListGetter(serviceManagerContainer);
            return new JiraAccountIdEngine(userListGetter);
        }

        private void CloneComponents(Issue issue, Issue newIssue)
        {
            foreach (var comp in issue.Components)
                newIssue.Components.Add(comp);
        }

        private void CloneVersions(Issue issue, Issue newIssue)
        {
            foreach (var v in issue.FixVersions)
                newIssue.FixVersions.Add(v);

            foreach (var a in issue.AffectsVersions)
                newIssue.AffectsVersions.Add(a);
        }

        private async Task CloneComments(Issue issue, Issue clonedIssue)
        {
            var comments = await issue.GetCommentsAsync();
            foreach (var c in comments)
                commentEngine.Execute(clonedIssue, c);
        }

        private async Task CloneWorkLogs(Issue issue, Issue clonedIssue)
        {
            var workLogs = await issue.GetWorklogsAsync();
            foreach (var w in workLogs)
                worklogEngine.Execute(clonedIssue, w, WorklogStrategy.AutoAdjustRemainingEstimate);
        }

        private async Task CloneWatchers(Issue issue, Issue clonedIssue)
        {
            var watchers = await issue.GetWatchersAsync();
            foreach (var w in watchers)
                await clonedIssue.AddWatcherAsync(SetUser(w.Username));
        }

        private string SetUser(string username)
        {
            var account = accountEngine.Execute(username);

            return account.AccountId;
        }

        private async Task CloneAttachments(Issue issue, Issue clonedIssue)
        {
            var attachments = await issue.GetAttachmentsAsync();

            foreach (var attach in attachments)
            {
                var byteArray = attach.DownloadData();

                var uAttachmentInfo = new UploadAttachmentInfo(attach.FileName, byteArray);

                clonedIssue.AddAttachment(uAttachmentInfo);
            }
        }

        private void CloneIssueType(Issue issue, Issue newIssue)
        {
            if (issueTypeMapping.TryGetValue(issue.Type.Id, out string mappedType))
                newIssue.Type = mappedType;
            else
                newIssue.Type = issue.Type;
        }

        private void CloneCustomFields(Issue issue, Issue newIssue)
        {
            return; //TODOPL

            foreach (var c in issue.CustomFields)
            {
                if (c.Id != "customfield_10018" &&   //parent_link
                    c.Id != "customfield_10019" &&  //rank
                    c.Id != "customfield_10000" &&  //development
                    c.Id != "customfield_10011" &&  //epic name
                    c.Id != "customfield_10014")    //epic_link
                {

                    if(customFieldsMapping.TryGetValue(c.Name, out string mappedName))
                        newIssue.CustomFields.Add(mappedName, c.Values);
                    else
                        newIssue.CustomFields.Add(c.Name, c.Values);
                }
            }
        }
    }
}
