using System;
using System.Collections.Generic;
using Atlassian.Jira;

namespace JiraTools.Model
{
    public class CreateIssueInfo
    {
        public CreateIssueInfo()
        {
            this.FixVersions = new List<string>();
            this.CustomFields = new List<CustomFieldInfo>();
            this.Components = new List<string>();
            this.Logged = new List<WorkLogInfo>();
        }

        public string ProjectKey { get; set; }

        public string ParentIssueKey { get; set; }

        public IssueType Type { get; set; }

        public IssuePriority Priority { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Assignee { get; set; }

        public string AssigneeUser { get; set; }

        public string OriginalEstimate { get; set; }

        public string RemainingEstimate { get; set; }

        public DateTime DueDate { get; set; }

        public List<string> FixVersions { get; set; }

        public List<CustomFieldInfo> CustomFields { get; set; }

        public List<string> Components { get; set; }

        public List<WorkLogInfo> Logged { get; set; }

        public List<Comment> CommentList { get; set; }

        public List<string> Attachments { get; set; }
    }
}
