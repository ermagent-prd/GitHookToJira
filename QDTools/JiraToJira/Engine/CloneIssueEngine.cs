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

        private readonly Dictionary<string, string> SPRINT_MAPPING = new Dictionary<string, string>()
        {
            {"MP Sprint 1", "186" },
            {"MP Sprint 2", "187" },
            {"MP Sprint 3", "188" },
        };

        private readonly Dictionary<string, string> issueTypeMapping = new Dictionary<string, string>()
        {
            { "10000", "10000" },    //epic
            { "10001", "10005" },    //story
            { "10002", "10023" },    //task
            { "10003", "10025" },    //subtask
            { "10014", "10001" },    //bug
            { "10004", "10001" }    //bug
        };


        private readonly Dictionary<string, string> customFieldsMapping = new Dictionary<string, string>();
        
        public CloneIssueEngine(
            ServiceManagerContainer requestFactory,
            AddWorklogEngine worklogEngine,
            AddCommentEngine commentEngine,
            AddAttachmentEngine attachmentEngineEngine)
        {
            this.worklogEngine = worklogEngine;
            this.commentEngine = commentEngine;

            //saving issues and user_account_search must be done on destination site
            this.requestFactory = JiraDestinantionRequestFactory();
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

            if (issue.Type.Id == "10001" || issue.Type.Id == "10002" || issue.Type.Id == "10003")
            {
                timeTrackingData = new IssueTimeTrackingData(
                    issue.TimeTrackingData.OriginalEstimate,
                    issue.TimeTrackingData.RemainingEstimate);
            }

            var fields = new CreateIssueFields(destProject)
            {
                TimeTrackingData = timeTrackingData
            };


            //da gestire
            if (relatedDev != null)
            {
                fields.ParentIssueKey = relatedDev.Key.Value;
            }
            else if (issue.Type.Id == "10003")  //sub-task
                return null;

            var newIssue = new Issue(this.requestFactory.Service, fields);

            newIssue.Summary = issue.Summary;
            newIssue.Description = issue.Description;

            CloneIssueType(issue, newIssue);

            newIssue.Priority = issue.Priority;

            if (issue.Type.Id != "10004")   // != BUG
                newIssue.DueDate = issue.DueDate;

            newIssue.Resolution = issue.Resolution;

            CloneVersions(issue, newIssue);
            CloneComponents(issue, newIssue);

            CloneCustomFields(issue, newIssue);

            if (issue.Type.Id == "10000")
                newIssue.CustomFields.Add("Epic Name", issue.CustomFields.FirstOrDefault(x => x.Name == "Epic Name").Values[0]);

            var clonedIssue = await newIssue.SaveChangesAsync();

            await CloneWorkLogs(issue, clonedIssue);

            await CloneComments(issue, clonedIssue);

            await CloneAttachments(issue, clonedIssue);

            await CloneWatchers(issue, clonedIssue);

            clonedIssue.Reporter = SetUser(issue.ReporterUser)?.AccountId;
            try
            {
                clonedIssue.SaveChanges();
            }
            catch
            {
                Console.WriteLine("Impossibile settare come reporter " + SetUser(issue.ReporterUser)?.DisplayName + " a partire dalla issue " + clonedIssue.Key.Value);
            }
            clonedIssue.Assignee = SetUser(issue.AssigneeUser)?.AccountId;
            try
            { 
            clonedIssue.SaveChanges();
            }
            catch
            {
                Console.WriteLine("Impossibile settare come assignee " + SetUser(issue.AssigneeUser)?.DisplayName + " a partire dalla issue " + clonedIssue.Key.Value);
            }

            return clonedIssue;
        }

        private ServiceManagerContainer JiraDestinantionRequestFactory()
        {
            return new ServiceManagerContainer(
                new JiraToJiraParamContainer()
                );
        }

        private JiraAccountIdEngine UserAccountEngine()
        {
            var serviceManagerContainer = new ServiceManagerContainer(new JiraToJiraParamContainer());
            var userListGetter = new UserListGetter(serviceManagerContainer);
            return new JiraAccountIdEngine(userListGetter);
        }

        private void CloneComponents(Issue issue, Issue newIssue)
        {
            if (issue.Type.Id != "10003")    //subtask
            {
                foreach (var comp in issue.Components)
                    newIssue.Components.Add(comp.Name);
            }
        }

        private void CloneVersions(Issue issue, Issue newIssue)
        {
            if (issue.Type.Id != "10003")    //subtask
            {
                foreach (var v in issue.FixVersions)
                    newIssue.FixVersions.Add(v.Name);
            }

            foreach (var a in issue.AffectsVersions)
                newIssue.AffectsVersions.Add(a.Name);
        }

        private async Task CloneComments(Issue issue, Issue clonedIssue)
        {
            var comments = await issue.GetCommentsAsync();
            foreach (var c in comments)
            {
                parseCommentByAuthor(c);
                commentEngine.Execute(clonedIssue, c);
            }
        }

        private void parseCommentByAuthor(Comment c)
        {
            if (c.Body.Length > 0 && !c.Body.StartsWith("[~accountId:"))
                c.Body = "[~accountId:" + c.Author + "]\n" + c.Body;
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
            {
                try
                {
                    await clonedIssue.AddWatcherAsync(SetUser(w).AccountId);
                }
                catch
                {
                    //do nothing
                }
            }
        }

        private JiraUser SetUser(JiraUser user)
        {
            if (user == null)
                return null;

            var account = accountEngine.Execute(user.DisplayName);
            return account;
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
                newIssue.Type = issue.Type.Id;
        }

        private void CloneCustomFields(Issue issue, Issue newIssue)
        {   
            foreach (var c in issue.CustomFields)
            {       
                if (CheckIfValidCustomFields(issue, c))
                { 
                    if (customFieldsMapping.TryGetValue(c.Name, out string mappedName))
                        newIssue.CustomFields.Add(mappedName, c.Values);
                    else
                        newIssue.CustomFields.Add(c.Name, c.Values);
                }
                else if(c.Id == "customfield_10020" && issue.Type.Id != "10003")
                {
                    //Sprint
                    var sprintink = issue.CustomFields.FirstOrDefault(x => x.Name == "Sprint");
                    if (sprintink != null && sprintink.Values[0] != "" && SPRINT_MAPPING.TryGetValue(sprintink.Values[0], out string sprint))
                            newIssue.CustomFields.Add("Sprint", sprint);
                }
            }

            newIssue.CustomFields.Add("StatusTmp", issue.Status.Name);
            newIssue.CustomFields.Add("OriginalKey", issue.Key.Value);
        }

        private bool CheckIfValidCustomFields(Issue issue, CustomFieldValue c)
        {
            bool result = true;

            if (c.Name == "StatusTmp")
                return false;

            if(issue.Type.Id == "10000")    //epic
            {
                result = c.Id != "customfield_10018" &&  //parent_link
                    c.Id != "customfield_10019" &&  //rank
                    c.Id != "customfield_10000" &&  //development
                    c.Id != "customfield_10011" &&  //epic name
                    c.Id != "customfield_10014" &&  //epic_link
                    c.Id != "customfield_10072" &&  //estimate_type
                    c.Id != "customfield_10020" &&  //sprint (è settato dopo)
                    c.Id != "customfield_10013" &&  //epic_color
                    c.Id != "customfield_10012" &&  //epic_status
                    c.Id != "customfield_10030" &&  //jde_module
                    !c.Name.Contains("GEMINI") &&
                    !c.Name.Contains("[CHART]") &&
                    c.Id != "customfield_10031";    //tasktype
            }
            else if (issue.Type.Id == "10002")  //task 
            {
                result = c.Id != "customfield_10018" &&  //parent_link
                    c.Id != "customfield_10019" &&  //rank
                    c.Id != "customfield_10028" &&  //story_points
                    c.Id != "customfield_10020" &&  //sprint (è settato dopo)
                    c.Id != "customfield_10000" &&  //epic_status
                    c.Id != "customfield_10014" &&  //epic_link
                    c.Id != "customfield_10031" &&  //tasktype
                    !c.Name.Contains("GEMINI") &&
                    !c.Name.Contains("[CHART]");    //[CHART]
            }
            else if (issue.Type.Id == "10001")  //story 
            {
                result = c.Id != "customfield_10018" &&  //parent_link
                    c.Id != "customfield_10019" &&  //rank
                    c.Id != "customfield_10028" &&  //story_points
                    c.Id != "customfield_10020" &&  //sprint (è settato dopo)
                    c.Id != "customfield_10000" &&  //epic_status
                    c.Id != "customfield_10014" &&  //epic_link
                    c.Id != "customfield_10031" &&  //tasktype
                    !c.Name.Contains("GEMINI") &&
                    !c.Name.Contains("[CHART]");    //[CHART]
            }
            else if (issue.Type.Id == "10003") //subtask
            {
                result = c.Id != "customfield_10018" &&  //parent_link
                    c.Id != "customfield_10019" &&  //rank
                    c.Id != "customfield_10028" &&  //story_points
                    c.Id != "customfield_10020" &&  //sprint (è settato dopo)
                    c.Id != "customfield_10000" &&  //epic_status
                    c.Id != "customfield_10014" &&  //epic_link
                    c.Id != "customfield_10030" &&  //JDE Module
                    !c.Name.Contains("GEMINI") &&
                    !c.Name.Contains("[CHART]");    //[CHART]
            }
            else if(issue.Type.Id == "10004")   //bug
            {
                result = c.Id != "customfield_10018" &&  //parent_link
                    c.Id != "customfield_10019" &&  //rank
                    c.Id != "customfield_10028" &&  //story_points
                    c.Id != "customfield_10020" &&  //sprint (è settato dopo)
                    c.Id != "customfield_10000" &&  //epic_status
                    c.Id != "customfield_10014" &&  //epic_link
                    c.Id != "customfield_10031" &&  //tasktype
                    c.Id != "customfield_10072" &&  //estimate type
                    c.Id != "customfield_10021" &&  //Flagged
                    //c.Id != "customfield_10030" &&  //JDE Module
                    !c.Name.Contains("[CHART]") &&
                    !c.Name.Contains("GEMINI") &&
                    c.Name != "Notes";    //[CHART]
            }


            return result;
        }
    }
}
