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
        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;
        private readonly TimeLogEngine timeLogEngine;

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

            jiraIssue.AffectVersions = new List<string>();
            jiraIssue.FixVersions = new List<string>();

            if (configurationSetup.Mapping.UAT_PRIORITY_MAPPING.TryGetValue(geminiIssue.Priority.ToLower(), out string priority))
                jiraIssue.Priority = priority;

            //Assignee
            var owner = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Owner");
            if (owner != null && owner.FormattedData != "")
                jiraIssue.Assignee = accountEngine.Execute(owner.FormattedData, configurationSetup.Jira.DefaultAccount).AccountId;

            //Load all issue's attachment
            jiraIssue.Attachments = descAttachments;
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments, configurationSetup.Gemini.ProjectUrl, configurationSetup.AttachmentDownloadedPath);

            //Load and map all gemini comments
            commentMapper.Execute(configurationSetup, jiraIssue, geminiIssue);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue, configurationSetup.Gemini.UatPrefix, configurationSetup.Mapping);

            //Related Dev
            SetRelatedDevelopment(jiraIssue, geminiIssue, configurationSetup.Gemini.ErmPrefix, configurationSetup.Mapping);

            //worklog
            jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries, configurationSetup.Jira.DefaultAccount);

            SetResources(jiraIssue, geminiIssue,configurationSetup.Jira.DefaultAccount);

            return jiraIssue;
        }




        #region Private        
        private void SetResources(CreateIssueInfo jiraIssue, IssueDto geminiIssue,string defaultAccount)
        {
            if (geminiIssue.Resources != null && geminiIssue.Resources.Count > 0)
            {
                jiraIssue.Watchers = new List<string>();

                foreach (var resource in geminiIssue.Resources)
                    jiraIssue.Watchers.Add(accountEngine.Execute(resource.User.Fullname, defaultAccount).AccountId);
            }
        }


        private void SetRelatedDevelopment(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string ermPrefix, JiraTools.Parameters.MappingConfiguration mapping)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.RELATED_DEVELOPMENT_LABEL);
            if (relatedDev != null)
            {
                jiraIssue.RelatedDevelopment = relatedDev.FormattedData;
                jiraIssue.RelatedDevelopmentId = ermPrefix + relatedDev.Entity.Data;
            }
        }

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string uatPrefix, JiraTools.Parameters.MappingConfiguration mapping)
        {
            string status = "";
            if (mapping.UAT_STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", mapping.UAT_STATUS_MAPPING_DEFAULT));

            //UAT Type
            var issueType = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.ISSUE_TYPE_LABEL);
            if (issueType != null)
            {
                string type;
                if (mapping.UAT_TYPE_MAPPING.TryGetValue(issueType.FormattedData.ToLower(), out type))
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Type", type));
            }

            //UAT Category
            if (geminiIssue.Components != null && geminiIssue.Components.Count > 0 && mapping.UAT_CATEGORY_MAPPING.TryGetValue(geminiIssue.Components[0].Entity.Name.ToLower(), out string category))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Category", category));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Category", mapping.UAT_CATEGORY_MAPPING_DEFAULT));

            //UAT Severity
            //map severity from gemini
            var severity = ParseSeverity(geminiIssue);
            if(mapping.UAT_SEVERITY_MAPPING.TryGetValue(severity.ToLower(), out string jiraSeverity))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Severity", jiraSeverity));

            //Fixed in build
            var fixedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.FIXED_IN_BUILD_LABEL);
            if (fixedBuild != null && fixedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixed In Build", fixedBuild.FormattedData));

            //Affected Build
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.AFFECTEDBUILD_LABEL);
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Affected Build", affectedBuild.FormattedData));

            //Save release build, if present, as Fix Versions
            //var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
            //if (release != null && release.FormattedData != "")
            //    jiraIssue.FixVersions.Add(release.FormattedData);

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("OriginalKey", uatPrefix + geminiIssue.Id.ToString()));

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
