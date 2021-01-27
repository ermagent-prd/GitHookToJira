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

        public UatIssueMapper(CommentMapper commentMapper, AttachmentGetter attachmentGetter, JiraAccountIdEngine accountEngine)
        {
            this.attachmentGetter = attachmentGetter;
            this.commentMapper = commentMapper;
            this.accountEngine = accountEngine;
        }


        public CreateIssueInfo Execute(IssueDto geminiIssue, string type)
        {
            
            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = "ER",          //TODO issue.Project.Code,
                Summary = geminiIssue.Title,
                Description = ParseCommentEngine.Execute(geminiIssue.Description) + DateTime.Now.ToString(),    //TODO recueprare le immagini se presenti?
                
                //TODO status
                //Priority = geminiIssue.Priority,
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h",
                RemainingEstimate = geminiIssue.RemainingTime,
                                
                Resolution = geminiIssue.Resolution
            };
            
            //TODO Fix Version??
            if (geminiIssue.FixedInVersion != "")
                jiraIssue.FixVersions.Add(geminiIssue.FixedInVersion);

            //TODO con campo temporaneo e regola post creazione
            jiraIssue.Assignee = accountEngine.Execute(geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname);   //TODO

            //Load all issue's attachment
            jiraIssue.Attachments = new List<string>();
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments);

            //Load and map all gemini comments
            commentMapper.Execute(jiraIssue, geminiIssue);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue);

            //TODO Components 
            var functionality = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == FUNCTIONALITY);
            if (functionality != null && functionality.FormattedData != "")
                jiraIssue.Components.Add("BSM"); //TODO manca ERMAS jiraIssue.Components.Add(functionality.FormattedData);

            //TODO geminiIssue.Visibility

            //TODO Related Dev
            SetRelatedDevelopment(jiraIssue, geminiIssue);

            return jiraIssue;
        }


        #region Private        

        private void SetRelatedDevelopment(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == RELATED_DEVELOPMENT);
            if (relatedDev != null)
            {
                jiraIssue.RelatedDevelopment = relatedDev.FormattedData;
                jiraIssue.RelatedDevelopmentId = GeminiConstants.ErmPrefix + relatedDev.Entity.Data;
            }
        }

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            //UAT Type
            var issueType = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == ISSUE_TYPE);
            if (issueType != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Type", issueType.Entity.Data));

            //UAT Category
            if (geminiIssue.Components != null && geminiIssue.Components.Count > 0)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Category", geminiIssue.Components[0].Entity.Name));

            //UAT Severity
            jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Severity", geminiIssue.Severity));

            //Fixed in build
            var fixedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == FIXED_IN_BUILD);
            if (fixedBuild != null && fixedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixed In Build", fixedBuild.FormattedData));

            //Affected Build
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == AFFECTEDBUILD);
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Affected Build", affectedBuild.FormattedData));

            //Save release build, if present
            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
            if (release != null && release.FormattedData != "")
                jiraIssue.FixVersions.Add(release.FormattedData);

        }

        #endregion
    }
}
