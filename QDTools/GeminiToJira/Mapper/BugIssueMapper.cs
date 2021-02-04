using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiToJira.Parameters;
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
        
        private Dictionary<string, string> BUG_TYPE_MAPPING = new Dictionary<string, string>()
        {
            { "Presentation", "Functional" },
            { "Engine", "Functional" },
            { "Sythesis", "Functional" },
            { "Other", "Functional" },
        };

        //private readonly Dictionary<string, string> STATUS_MAPPING = new Dictionary<string, string>()
        //{
        //    { "Assigned",   "Assigned" },
        //    { "In Progress",   "In Progress" },
        //    { "Cancelled",   "Cancelled" },
        //    { "Testing",   "in Progress" },
        //    { "Fixed",  "Fixed" },
        //};


        private const string AFFECTEDBUILD = "FoundInBuild";
        private const string DEVELOPMENT_RELEASE_KEY = "Release Version";
        private const string FUNCTIONALITY = "Functionality";
        private const string PROJECT_MODULE = "Product Module";
        private const string RELATED_DEVELOPMENT = "Development";
        private const string ISSUE_TYPE = "IssueType";
        private const string FIXED_IN_BUILD = "FixedInBuild";

        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;

        public BugIssueMapper(
            CommentMapper commentMapper, 
            AttachmentGetter attachmentGetter, 
            JiraAccountIdEngine accountEngine, 
            ParseCommentEngine parseCommentEngine)
        {
            this.attachmentGetter = attachmentGetter;
            this.commentMapper = commentMapper;
            this.accountEngine = accountEngine;
            this.parseCommentEngine = parseCommentEngine;
        }


        public CreateIssueInfo Execute(IssueDto geminiIssue, string type, string projectCode)
        {

            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = projectCode,
                Summary = geminiIssue.Title,
                Description = parseCommentEngine.Execute(geminiIssue.Description) + " " + DateTime.Now.ToString(),    //TODO recueprare le immagini se presenti?
                Priority = geminiIssue.Priority,
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h " + geminiIssue.EstimatedMinutes + "m",
                RemainingEstimate = geminiIssue.RemainingTime,

                //TODO Resolution = geminiIssue.Resolution
            };


            //it's the same from the one from Gemini
            string status = "";
            if (geminiIssue.Status != null && geminiIssue.Status != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", geminiIssue.Status));

            SetAffectedVersion(geminiIssue, jiraIssue);
            SetFixVersion(geminiIssue, jiraIssue);
                        
            //Assignee
            if (geminiIssue.Resources.Count > 0)
                jiraIssue.Assignee = accountEngine.Execute(geminiIssue.Resources.First().Entity.Fullname).AccountId;

            //Load all issue's attachment
            jiraIssue.Attachments = new List<string>();
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments);
            
            //Load and map all gemini comments
            commentMapper.Execute(jiraIssue, geminiIssue);
            
            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue);
            
            //For components use
            SetRelatedDevelopment(jiraIssue, geminiIssue);

            return jiraIssue;
        }




        #region Private        
        private void SetRelatedDevelopment(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == PROJECT_MODULE);
            if (relatedDev != null)
            {
                jiraIssue.RelatedDevelopment = relatedDev.FormattedData;
                jiraIssue.RelatedDevelopmentId = GeminiConstants.ErmPrefix + relatedDev.Entity.Data;
            }
        }

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            var owner = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Owner");
            if (owner != null && owner.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Owner", accountEngine.Execute(owner.FormattedData).AccountId));

            
            var bugType = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "BugType");
            string jiraBugType;
            if (bugType != null && bugType.FormattedData != "" && BUG_TYPE_MAPPING.TryGetValue(bugType.FormattedData, out jiraBugType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Type", jiraBugType));


            //Bug Severity
            if (geminiIssue.Severity != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Severity", ParseSeverity(geminiIssue)));

            //Fixing Date
            var fixingDate = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Fixing Date");
            if(fixingDate != null && fixingDate.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixing Date", fixingDate.Entity.DateData.Value.ToString("yyyy-M-d")));

            //Affected Build
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == AFFECTEDBUILD);
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Affected Build", affectedBuild.FormattedData));

            //Fixed in build
            var fixedInBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "FixedInBuild");
            if(fixedInBuild != null && fixedInBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixed In Build", fixedInBuild.FormattedData));

            //"CauseType"
            var causeType = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "CauseType");
            if (causeType != null && causeType.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Cause Type", parseCommentEngine.Execute(causeType.FormattedData)));

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
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action type", parseCommentEngine.Execute(suggestedActionsType.FormattedData)));

            //"SuggestedActions"
            var suggestedActions = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "SuggestedActions");
            if (suggestedActions != null && suggestedActions.FormattedData.Length > 3)  //la string può contenere anche solo \n, \r, \nr, \rn
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action", parseCommentEngine.Execute(suggestedActions.FormattedData)));

            //"Notes"
            var notes = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "Notes");
            if (notes != null && notes.FormattedData.Length > 3)  //la string può contenere anche solo \n, \r, \nr, \rn
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Notes", parseCommentEngine.Execute(notes.FormattedData)));
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

        private void SetFixVersion(IssueDto geminiIssue, CreateIssueInfo jiraIssue)
        {
            jiraIssue.FixVersions = new List<string>();

            var fixVersion = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "Deliverable Versions");
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
                return geminiIssue.Severity.Substring(geminiIssue.Severity.IndexOf("-") + 2);
            else
                return geminiIssue.Severity;
        }


        #endregion
    }
}
