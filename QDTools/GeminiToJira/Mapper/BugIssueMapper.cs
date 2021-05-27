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
    public class BugIssueMapper
    {
        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;
        private readonly TimeLogEngine timeLogEngine;

        public BugIssueMapper(
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


        public CreateIssueInfo Execute(GeminiToJiraParameters configurationSetup, IssueDto geminiIssue, string type, string projectCode, string ermPrefix)
        {
            var descAttachments = new List<string>();

            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = projectCode,   
                Summary = geminiIssue.Title.TrimEnd() == "" ? geminiIssue.IssueKey : geminiIssue.Title.TrimEnd(), //for bug without title
                Description = parseCommentEngine.Execute(geminiIssue.Description, "desc", descAttachments, configurationSetup.AttachmentDownloadedPath),
                Type = type,
                //OriginalEstimate = geminiIssue.EstimatedHours + "h " + geminiIssue.EstimatedMinutes + "m",
                //RemainingEstimate = geminiIssue.RemainingTime,
            };

            if (configurationSetup.Mapping.BUG_PRIORITY_MAPPING.TryGetValue(geminiIssue.Priority.ToLower(), out string priority))
                jiraIssue.Priority = priority;

            SetAffectedVersion(geminiIssue, jiraIssue);
            SetFixVersion(geminiIssue, jiraIssue, configurationSetup.Mapping);

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
            LoadCustomFields(jiraIssue, geminiIssue, ermPrefix, configurationSetup);
            
            //For components use
            SetRelatedDevelopment(jiraIssue, geminiIssue, configurationSetup.Gemini.ErmPrefix, configurationSetup.Mapping);

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

        private void SetRelatedDevelopment(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string ermPrefix, JiraTools.Parameters.MappingConfiguration mapping)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.BUG_PROJECT_MODULE);
            if (relatedDev != null)
            {
                jiraIssue.RelatedDevelopment = relatedDev.FormattedData;
                jiraIssue.RelatedDevelopmentId = ermPrefix + relatedDev.Entity.Data;
            }
        }

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string ermPrefix, GeminiToJiraParameters configurationSetup)
        {
            var mapping = configurationSetup.Mapping;

            //recuperare fix versions (da capire come fare)
            

            if (mapping.BUG_STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out string status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));
            else if (mapping.UAT_STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));

            var bugType = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "BugType");
            string jiraBugType;
            if (bugType != null && bugType.FormattedData != "" && mapping.BUG_TYPE_MAPPING.TryGetValue(bugType.FormattedData, out jiraBugType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Type", jiraBugType));

            //Bug Severity
            if (!string.IsNullOrWhiteSpace(geminiIssue.Severity))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Severity", ParseSeverity(geminiIssue)));



            //Fixing Date
            var fixingDate = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Fixing Date");
            if(fixingDate != null && fixingDate.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixing Date", fixingDate.Entity.DateData.Value.ToString("yyyy-M-d")));

            //Affected Build
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.BUG_AFFECTEDBUILD_LABEL);
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Affected Build", affectedBuild.FormattedData));

            //Fixed in build
            var fixedInBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "FixedInBuild");
            if(fixedInBuild != null && fixedInBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixed In Build", fixedInBuild.FormattedData));

            //"CauseType"
            var causeType = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "CauseType");
            if (causeType != null && causeType.FormattedData != "")
            {
                if(mapping.BUG_CAUSE_TYPE_MAPPING.TryGetValue(parseCommentEngine.Execute(causeType.FormattedData).ToLower(), out string cause))
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Cause Type", cause));
                else
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Cause Type", mapping.BUG_CAUSE_TYPE_DEFAULT));
            }

            //Cause description
            var causeDesc = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "Cause");
            if (causeDesc != null && causeDesc.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Cause Description", parseCommentEngine.Execute(causeDesc.FormattedData)));

            //Fix description
            var fixingDesc = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "Fixing Description");
            if (fixingDesc != null && fixingDesc.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug fix description", parseCommentEngine.Execute(fixingDesc.FormattedData)));

            //"SuggestedActionsType"
            var suggestedActionsType = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "SuggestedActionsType");
            if (suggestedActionsType != null && suggestedActionsType.FormattedData != "")
            {
                if(mapping.BUG_SUGGESTED_ACTION_TYPE_MAPPING.TryGetValue(parseCommentEngine.Execute(suggestedActionsType.FormattedData).ToLower(), out string suggestedAction))
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action type", suggestedAction));
                else
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action type", mapping.BUG_SUGGESTED_ACTION_TYPE_DEFAULT));
            }

            //"SuggestedActions"
            var suggestedActions = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "SuggestedActions");
            if (suggestedActions != null && suggestedActions.FormattedData.Length > 3)  //la string può contenere anche solo \n, \r, \nr, \rn
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action", parseCommentEngine.Execute(suggestedActions.FormattedData)));

            //"Notes"

            var notes = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "Notes");
            if (notes != null && !string.IsNullOrWhiteSpace(notes.FormattedData) && !notes.FormattedData.Equals("\r\n\r\n"))  //la string può contenere anche solo \n, \r, \nr, \rn
                jiraIssue.Description += notes.FormattedData;


//                jiraIssue.CustomFields.Add(new CustomFieldInfo("Notes", parseCommentEngine.Execute(notes.FormattedData)));

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("OriginalKey", ermPrefix + geminiIssue.Id.ToString()));

            var esup = getGeminiEsup(geminiIssue,configurationSetup);
            if (esup != null)
                jiraIssue.CustomFields.Add(esup);
        }

        private CustomFieldInfo getGeminiEsup(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            var esup = geminiIssue.Hierarchy.FirstOrDefault(i => i.Value.EscapedProjectCode == "ESUP");

            if (esup == null)
                return null;

            string esupurl = configurationSetup.Gemini.ESUPItemLinkUrl + esup.Value.Id;

            return new CustomFieldInfo("Gemini ESUP", esupurl);
            
        }

        private void SetAffectedVersion(IssueDto geminiIssue, CreateIssueInfo jiraIssue)
        {
            jiraIssue.AffectVersions = new List<string>();

            if (geminiIssue.AffectedVersionNumbers != "")
            {
                var affectedVersions = ExtractVersions(geminiIssue.AffectedVersionNumbers);
                foreach (var version in affectedVersions)
                    jiraIssue.AffectVersions.Add(version);
            }
        }

        private void SetFixVersion(IssueDto geminiIssue, CreateIssueInfo jiraIssue, JiraTools.Parameters.MappingConfiguration mapping)
        {
            jiraIssue.FixVersions = new List<string>();

            var fixVersion = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.BUG_DELIVERABLE_VERSIONS);
            if (fixVersion != null && fixVersion.FormattedData != "")
            {
                var fixVersions = ExtractVersions(fixVersion.FormattedData);
                foreach (var version in fixVersions)
                    jiraIssue.FixVersions.Add(version.TrimStart().TrimEnd());
            }
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
