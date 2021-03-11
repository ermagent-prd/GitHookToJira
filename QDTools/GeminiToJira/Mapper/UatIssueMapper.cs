using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiToJira.Parameters;
using GeminiToJira.Parameters.Import;
using GeminiTools.Items;
using GeminiTools.Parameters;
using JiraTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiToJira.Mapper
{
    public class UatIssueMapper
    {
        private const string AFFECTEDBUILD = "AffectedBuild";
        private const string DEVELOPMENT_RELEASE_KEY = "Release Version";
        private const string FUNCTIONALITY = "Functionality";
        private const string RELATED_DEVELOPMENT = "Development";
        private const string ISSUE_TYPE = "IssueType";
        private const string FIXED_IN_BUILD = "FixedInBuild";

        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;
        private readonly TimeLogEngine timeLogEngine;

        private readonly Dictionary<string, string> STATUS_MAPPING = new Dictionary<string, string>()
        {
            { "assigned",   "To Do" },
            { "in progress",   "In progress" },
            { "fixed",  "Ready for test" },
            { "testing",   "Testing" },
            { "closed",  "Closed" },
            { "rejected",   "Rejected" },
            { "cancelled",   "Cancelled" },
            { "in Backlog",  "In Backlog" },
        };
        private readonly string STATUS_MAPPING_DEFAULT = "To Do";

        private readonly Dictionary<string, string> PRIORITY_MAPPING = new Dictionary<string, string>()
        {
            { "low", "Low" },
            { "medium", "Medium" },
            { "high", "High" },
        };

        private readonly Dictionary<string, string> SEVERITY_MAPPING = new Dictionary<string, string>()
        {
            { "low", "Trivial" },
            { "medium", "Minor" },
            { "high", "Major" },
            { "highest", "Blocking" },
        };

        private readonly Dictionary<string, string> CATEGORY_MAPPING = new Dictionary<string, string>()
        {
            {"calcoli", "Functional"},
            {"interfaccia", "Usability"},
            {"usabilità", "Usability"},
            {"efficenza", "Performance"},
            {"suggerimento", "Usability"},
            {"localizzazione", "Usability"},
            {"messaggio di errore", "Functional"},
            {"dal", "Functional"}
        };
        private readonly string CATEGORY_MAPPING_DEFAULT = "Functional";


        private readonly Dictionary<string, string> TYPE_MAPPING = new Dictionary<string, string>()
        {
            { "defect",   "Defect" },
            { "investigation",   "Investigation" },
            { "enhancement",   "Enhancement" },
            { "enanchement",   "Enhancement" },
            { "regression",   "Regression" },
            { "setup",   "Setup" },
            { "change request",   "Change request" },
            { "new feature",   "New Feature" },
            { "missing functionality",   "Missing Functionality" },
        };


        public UatIssueMapper(
            CommentMapper commentMapper, 
            AttachmentGetter attachmentGetter, 
            JiraAccountIdEngine accountEngine, 
            ParseCommentEngine parseCommentEngine,
            TimeLogEngine timeLogEngine)
        {
            this.attachmentGetter = attachmentGetter;
            this.commentMapper = commentMapper;
            this.accountEngine = accountEngine;
            this.parseCommentEngine = parseCommentEngine;
            this.timeLogEngine = timeLogEngine;
        }


        public CreateIssueInfo Execute(GeminiToJiraParameters configurationSetup, IssueDto geminiIssue, string type, string projectCode)
        {
            var descAttachments = new List<string>();

            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = projectCode,
                Summary = geminiIssue.Title.TrimEnd(),
                Description = parseCommentEngine.Execute(geminiIssue.Description, "desc", descAttachments, configurationSetup.AttachmentDownloadedPath) + " " + DateTime.Now.ToString(),
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h " + geminiIssue.EstimatedMinutes + "m",
                RemainingEstimate = geminiIssue.RemainingTime
            };

            string status = "";
            if (STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", STATUS_MAPPING_DEFAULT));

            jiraIssue.AffectVersions = new List<string>();
            jiraIssue.FixVersions = new List<string>();

            if (PRIORITY_MAPPING.TryGetValue(geminiIssue.Priority.ToLower(), out string priority))
                jiraIssue.Priority = priority;

            //Assignee
            var owner = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Owner");
            if (owner != null && owner.FormattedData != "")
                jiraIssue.Assignee = accountEngine.Execute(owner.FormattedData).AccountId;

            //Load all issue's attachment
            jiraIssue.Attachments = descAttachments;
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments, configurationSetup.Gemini.ProjectUrl, configurationSetup.AttachmentDownloadedPath);

            //Load and map all gemini comments
            commentMapper.Execute(configurationSetup, jiraIssue, geminiIssue);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue, configurationSetup.Gemini.UatPrefix);

            //Related Dev
            SetRelatedDevelopment(jiraIssue, geminiIssue, configurationSetup.Gemini.ErmPrefix);

            //worklog
            jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries);

            SetResources(jiraIssue, geminiIssue);

            return jiraIssue;
        }




        #region Private        
        private void SetResources(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Resources != null && geminiIssue.Resources.Count > 0)
            {
                jiraIssue.Resources = new List<string>();

                foreach (var resource in geminiIssue.Resources)
                    jiraIssue.Resources.Add(accountEngine.Execute(resource.User.Fullname).AccountId);
            }
        }


        private void SetRelatedDevelopment(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string ermPrefix)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == RELATED_DEVELOPMENT);
            if (relatedDev != null)
            {
                jiraIssue.RelatedDevelopment = relatedDev.FormattedData;
                jiraIssue.RelatedDevelopmentId = ermPrefix + relatedDev.Entity.Data;
            }
        }

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string uatPrefix)
        {   
            //UAT Type
            var issueType = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == ISSUE_TYPE);
            if (issueType != null)
            {
                string type;
                if (TYPE_MAPPING.TryGetValue(issueType.FormattedData.ToLower(), out type))
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Type", type));
            }

            //UAT Category
            if (geminiIssue.Components != null && geminiIssue.Components.Count > 0 && CATEGORY_MAPPING.TryGetValue(geminiIssue.Components[0].Entity.Name.ToLower(), out string category))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Category", category));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Category", CATEGORY_MAPPING_DEFAULT));

            //UAT Severity
            //map severity from gemini
            var severity = ParseSeverity(geminiIssue);
            if(SEVERITY_MAPPING.TryGetValue(severity.ToLower(), out string jiraSeverity))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Severity", jiraSeverity));

            //Fixed in build
            var fixedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == FIXED_IN_BUILD);
            if (fixedBuild != null && fixedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixed In Build", fixedBuild.FormattedData));

            //Affected Build
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == AFFECTEDBUILD);
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Affected Build", affectedBuild.FormattedData));

            //Save release build, if present, as Fix Versions
            //var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
            //if (release != null && release.FormattedData != "")
            //    jiraIssue.FixVersions.Add(release.FormattedData);

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("Gemini", uatPrefix + geminiIssue.Id.ToString()));

        }

        private static string ParseSeverity(IssueDto geminiIssue)
        {
            if (geminiIssue.Severity.Contains('-'))
                return geminiIssue.Severity.Substring(geminiIssue.Severity.IndexOf("-") + 2);
            else
                return geminiIssue.Severity;
        }

        #endregion
    }
}
