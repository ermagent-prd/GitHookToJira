using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiToJira.Parameters.Import;
using GeminiTools.Items;
using JiraTools.Model;

namespace GeminiToJira.Mapper
{
    public class BugIssueMapper
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

        public BugIssueMapper(
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
            string epicLink)
        {
            var descAttachments = new List<string>();

            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = projectCode,
                Summary = geminiIssue.Title.TrimEnd() == "" ? geminiIssue.IssueKey : geminiIssue.Title.TrimEnd(), //for bug without title
                Description = parseCommentEngine.Execute(geminiIssue.Description, "desc", descAttachments, configurationSetup.AttachmentDownloadedPath),
                Type = type
            };

            if (configurationSetup.Mapping.BUG_PRIORITY_MAPPING.TryGetValue(geminiIssue.Priority.ToLower(), out string priority))
                jiraIssue.Priority = priority;

            this.affectedVersionEngine.Execute(geminiIssue, jiraIssue);

            SetBugFixVersion(geminiIssue, jiraIssue, configurationSetup.Mapping);

            //Assignee
            this.assigneeEngine.Execute(geminiIssue, jiraIssue, configurationSetup.Jira.DefaultAccount);

            //Load all issue's attachment
            jiraIssue.Attachments = descAttachments;
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments, configurationSetup.Gemini.ProjectUrl, configurationSetup.AttachmentDownloadedPath);

            //Load and map all gemini comments
            commentMapper.Execute(configurationSetup, jiraIssue, geminiIssue);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue, ermPrefix, configurationSetup);

            //Epic Link
            SetEpicLink(jiraIssue, epicLink);

            //worklog
            jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries, configurationSetup.Jira.DefaultAccount);

            //watchers
            this.watchersEngine.Execute(jiraIssue, geminiIssue);

            return jiraIssue;
        }

        #endregion



        #region Private        

        private void SetEpicLink(CreateIssueInfo jiraIssue, string epicLink)
        {
            this.customeFieldEngine.Execute(jiraIssue, "Epic Link", epicLink);
        }

        private void LoadCustomFields(
            CreateIssueInfo jiraIssue, 
            IssueDto geminiIssue, 
            string ermPrefix, 
            GeminiToJiraParameters configurationSetup)
        {
            var mapping = configurationSetup.Mapping;


            //status
            if (mapping.BUG_STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out string status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));

            //Bug Type    
            var bugType = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "BugType");
                string jiraBugType;
            if (bugType != null && bugType.FormattedData != "" && mapping.BUG_TYPE_MAPPING.TryGetValue(bugType.FormattedData, out jiraBugType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Type", jiraBugType));

            //Bug Severity
            string severityValue = string.IsNullOrWhiteSpace(geminiIssue.Severity) ?
                configurationSetup.Mapping.BUG_SEVERITY_MAPPING_DEFAULT :
                geminiIssue.Severity;

            if (mapping.BUG_SEVERITY_MAPPING.TryGetValue(severityValue, out string mappedSeverity))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Severity", mappedSeverity));


            //Fixing Date
            var fixingDate = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Fixing Date");
            if (fixingDate != null && fixingDate.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixing Date", fixingDate.Entity.DateData.Value.ToString("yyyy-M-d")));

            //AffectedBuild
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "FoundInBuild");
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Affected Build", affectedBuild .FormattedData));

            //Product module
            var productModule = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "Product Module");
            if (productModule != null && productModule.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("OriginalProductModule", productModule.FormattedData));

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
                if (mapping.BUG_SUGGESTED_ACTION_TYPE_MAPPING.TryGetValue(parseCommentEngine.Execute(suggestedActionsType.FormattedData).ToLower(), out string suggestedAction))
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

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("OriginalKey", ermPrefix + geminiIssue.Id.ToString()));

            var esup = getGeminiEsup(geminiIssue,configurationSetup);
            if (esup != null)
                jiraIssue.CustomFields.Add(esup);
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


        private void SetBugFixVersion(IssueDto geminiIssue, CreateIssueInfo jiraIssue, JiraTools.Parameters.MappingConfiguration mapping)
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
