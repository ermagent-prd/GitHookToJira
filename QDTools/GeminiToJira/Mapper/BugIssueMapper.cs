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
        
        private readonly Dictionary<string, string> BUG_TYPE_MAPPING = new Dictionary<string, string>()
        {
            { "Presentation", "Functional" },
            { "Engine", "Functional" },
            { "Sythesis", "Functional" },
            { "Other", "Functional" },
        };

        private readonly Dictionary<string, string> BUG_CAUSE_TYPE_MAPPING = new Dictionary<string, string>()
        {
            {"development mistake","Development mistake" },
            {"analysis mistake","Analysis mistake" },
            {"sql mistake","Development mistake" },
            {"presentation mistake","Presentation mistake" },
            {"enhancement","Enhancement" },
            {"missing development","Missing development" },
            {"regression","Regression" },
            {"refactoring","Refactoring" },
            {"licensing","Licensing" },
            {"complexity","Code complexity" },
            {"db upgrade","Datasource upgrade" },
            {"devops","Regression" },
            {"duplicate code","Development mistake" },
            {"performance","Performance" }
        };

        private readonly string BUG_CAUSE_TYPE_DEFAULT = "Other";

        private readonly Dictionary<string, string> BUG_SUGGESTED_ACTION_TYPE_MAPPING = new Dictionary<string, string>()
        {
            {"more attention", "Integration test" },
            {"more test", "Integration test" },
            {"unit test", "Test automation" },
            {"tca", "Integration test" },
            {"refactoring", "Refactoring" },
            {"presentation test", "Integration test" },
            {"missing documentation", "Documentation" },
            {"uat", "Integration test" },
            {"database test", "Integration test" },
            {"enancement management", "Processes improvement" },
            {"performance test", "Performance test" },
            {"eagle Integration", "Integration test" },
            {"licence test", "Licence test" },
            {"use entity framework", "Refactoring" },
            {"sql test", "Integration test" }
        };

        private readonly string BUG_SUGGESTED_ACTION_TYPE_DEFAULT = "Other";



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


        public CreateIssueInfo Execute(IssueDto geminiIssue, string type, string projectCode)
        {
            var descAttachments = new List<string>();

            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = projectCode,//for bug without title
                Summary = geminiIssue.Title.TrimEnd() == "" ? geminiIssue.IssueKey : geminiIssue.Title.TrimEnd(),
                Description = parseCommentEngine.Execute(geminiIssue.Description, "desc", descAttachments) + " " + DateTime.Now.ToString(),
                Priority = geminiIssue.Priority,
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h " + geminiIssue.EstimatedMinutes + "m",
                RemainingEstimate = geminiIssue.RemainingTime,
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
            jiraIssue.Attachments = descAttachments;
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments);
            
            //Load and map all gemini comments
            commentMapper.Execute(jiraIssue, geminiIssue);
            
            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue);
            
            //For components use
            SetRelatedDevelopment(jiraIssue, geminiIssue);

            //For worklog
            jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries);

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
            {
                if(BUG_CAUSE_TYPE_MAPPING.TryGetValue(parseCommentEngine.Execute(causeType.FormattedData).ToLower(), out string cause))
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Cause Type", cause));
                else
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug Cause Type", BUG_CAUSE_TYPE_DEFAULT));
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
                if(BUG_SUGGESTED_ACTION_TYPE_MAPPING.TryGetValue(parseCommentEngine.Execute(suggestedActionsType.FormattedData).ToLower(), out string suggestedAction))
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action type", suggestedAction));
                else
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action type", BUG_SUGGESTED_ACTION_TYPE_DEFAULT));
            }

            //"SuggestedActions"
            var suggestedActions = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "SuggestedActions");
            if (suggestedActions != null && suggestedActions.FormattedData.Length > 3)  //la string può contenere anche solo \n, \r, \nr, \rn
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Bug suggested action", parseCommentEngine.Execute(suggestedActions.FormattedData)));

            //"Notes"
            var notes = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == "Notes");
            if (notes != null && notes.FormattedData.Length > 3)  //la string può contenere anche solo \n, \r, \nr, \rn
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Notes", parseCommentEngine.Execute(notes.FormattedData)));

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("Gemini", GeminiConstants.ErmBugPrefix + geminiIssue.Id.ToString()));
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
                return geminiIssue.Severity.Substring(geminiIssue.Severity.IndexOf("-") + 1).TrimStart();
            else
                return geminiIssue.Severity;
        }


        #endregion
    }
}
