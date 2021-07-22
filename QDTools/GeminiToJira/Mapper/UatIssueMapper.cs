using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiToJira.Parameters;
using GeminiToJira.Parameters.Import;
using GeminiTools.Items;
using GeminiTools.Parameters;
using JiraTools.Model;
using JiraTools.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiToJira.Mapper
{

    public class UATIssueMapper
    {
        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;
        private readonly TimeLogEngine timeLogEngine;
        private readonly AddCustomFieldEngine customeFieldEngine;
        private readonly AddWatchersEngine watchersEngine;
        private readonly AssigneeEngine assigneeEngine;
        private readonly AffectedVersionsEngine affectedVersionEngine;

        public UATIssueMapper(
            CommentMapper commentMapper, 
            AttachmentGetter attachmentGetter, 
            JiraAccountIdEngine accountEngine, 
            ParseCommentEngine parseCommentEngine,
            TimeLogEngine timeLogEngine,
            AddCustomFieldEngine customeFieldEngine,
            AddWatchersEngine watchersEngine,
            AssigneeEngine assigneeEngine,
            AffectedVersionsEngine affectedVersionEngine)
        {
            this.attachmentGetter = attachmentGetter;
            this.commentMapper = commentMapper;
            this.accountEngine = accountEngine;
            this.parseCommentEngine = parseCommentEngine;
            this.timeLogEngine = timeLogEngine;
            this.customeFieldEngine = customeFieldEngine;
            this.watchersEngine = watchersEngine;
            this.assigneeEngine = assigneeEngine;
            this.affectedVersionEngine = affectedVersionEngine;
        }

        #region Public methods

        public CreateIssueInfo Execute(
            GeminiToJiraParameters configurationSetup, 
            IssueDto geminiIssue, 
            string type, 
            string projectCode, 
            string ermPrefix,
            string epicLink,
            Issue relatedDev)
        {
            var descAttachments = new List<string>();

            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = projectCode,
                Summary = geminiIssue.Title.TrimEnd() == "" ? geminiIssue.IssueKey : geminiIssue.Title.TrimEnd(), //for bug without title
                Description = parseCommentEngine.Execute(geminiIssue.Description, "desc", descAttachments, configurationSetup.AttachmentDownloadedPath),
                Type = type
            };

            //Priority
            if (configurationSetup.Mapping.BUG_PRIORITY_MAPPING.TryGetValue(geminiIssue.Priority.ToLower(), out string priority))
                jiraIssue.Priority = priority;

            //Affectd version
            this.affectedVersionEngine.Execute(geminiIssue, jiraIssue);

            //Assignee
            this.assigneeEngine.Execute(geminiIssue, jiraIssue, configurationSetup.Jira.DefaultAccount);

            //Load all issue's attachment
            jiraIssue.Attachments = descAttachments;
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments, configurationSetup.Gemini.ProjectUrl, configurationSetup.AttachmentDownloadedPath);

            //Load and map all gemini comments
            commentMapper.Execute(configurationSetup, jiraIssue, geminiIssue);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue, ermPrefix, configurationSetup, relatedDev);

            //Epic Link
            SetEpicLink(jiraIssue, epicLink);

            //For components use
            SetRelatedDevelopment(jiraIssue, geminiIssue, configurationSetup.Gemini.ErmPrefix, configurationSetup.Mapping);

            //worklog
            jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries, configurationSetup.Jira.DefaultAccount);

            //watchers
            this.watchersEngine.Execute(jiraIssue, geminiIssue);

            return jiraIssue;
        }

        #endregion



        #region Private        

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

        private void SetEpicLink(CreateIssueInfo jiraIssue, string epicLink)
        {
            this.customeFieldEngine.Execute(jiraIssue, "Epic Link", epicLink);
        }

        private void LoadCustomFields(
            CreateIssueInfo jiraIssue, 
            IssueDto geminiIssue, 
            string ermPrefix, 
            GeminiToJiraParameters configurationSetup,
            Issue relatedDev)
        {
            var mapping = configurationSetup.Mapping;

            //Status
            string status = null;
            if (mapping.UAT_STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));

            //Bug Category
            jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Category", "Pre-release"));

            //Bug Type
            var uatComponent = geminiIssue.Components.FirstOrDefault();
            string jiraUatBugType;
            if (uatComponent != null && !string.IsNullOrWhiteSpace(uatComponent.Entity.Name) && mapping.UAT_BUG_TYPE_MAPPING.TryGetValue(uatComponent.Entity.Name, out jiraUatBugType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Type", jiraUatBugType));

            //JdeModule
            setJdeModule(relatedDev, jiraIssue);

            //development line
            setDevelopmentLine(relatedDev, jiraIssue);

            //Bug Severity
            string severityValue = string.IsNullOrWhiteSpace(geminiIssue.Severity) ?
                configurationSetup.Mapping.UAT_SEVERITY_MAPPING_DEFAULT :
                geminiIssue.Severity;

            //Severity
            if (mapping.UAT_SEVERITY_MAPPING.TryGetValue(severityValue, out string mappedSeverity))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Severity", mappedSeverity));

            //Affected build
            var uatAffectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.UAT_AFFECTEDBUILD_LABEL);
            if (uatAffectedBuild != null && uatAffectedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Affected Build", uatAffectedBuild.FormattedData));

            //Fixed in build
            var fixedInBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "FixedInBuild");
            if (fixedInBuild != null && fixedInBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixed In Build", fixedInBuild.FormattedData));


            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("OriginalKey", ermPrefix + geminiIssue.Id.ToString()));

            var esup = getGeminiEsup(geminiIssue,configurationSetup);
            if (esup != null)
                jiraIssue.CustomFields.Add(esup);
        }


        private void setJdeModule(Issue relatedDev, CreateIssueInfo jiraIssue)
        {
            if (relatedDev == null)
                return;

            var jdeModule = relatedDev.CustomFields.FirstOrDefault(x => x.Name == "JDE Module");

            if (jdeModule == null || !jdeModule.Values.Any())
                return;

            string value = jdeModule.Values.FirstOrDefault();

            jiraIssue.CustomFields.Add(new CustomFieldInfo("JDE Module", value));
        }

        private void setDevelopmentLine(Issue relatedDev, CreateIssueInfo jiraIssue)
        {
            if (relatedDev == null)
                return;

            var jdeModule = relatedDev.CustomFields.FirstOrDefault(x => x.Name == "Development Line");

            if (jdeModule == null || !jdeModule.Values.Any())
                return;

            string value = jdeModule.Values.FirstOrDefault();

            jiraIssue.CustomFields.Add(new CustomFieldInfo("Development Line", value));
        }


        private CustomFieldInfo getGeminiEsup(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            var esup = geminiIssue.Hierarchy.FirstOrDefault(i => i.Value.EscapedProjectCode == "ESUP");

            if (esup == null)
                return null;

            string esupurl = configurationSetup.Gemini.ESUPItemLinkUrl + esup.Value.Id;

            return new CustomFieldInfo("Gemini ESUP", esupurl);
            
        }


        private List<string> ExtractVersions(string versionList)
        {
            var list = versionList.Split(',');
            return list.Select(x => x.TrimEnd().TrimStart()).ToList();
        }

        private static string ParseSeverity(IssueDto geminiIssue)
        {
            if (geminiIssue.Severity.Contains('-'))
                return geminiIssue.Severity.Substring(geminiIssue.Severity.IndexOf("-") + 1).TrimStart();
            else
                return geminiIssue.Severity;
        }


        #endregion
    }
}
