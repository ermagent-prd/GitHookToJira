using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiTools.Items;
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


        public UatIssueMapper()
        {
        }

        public CreateIssueInfo Execute(IssueDto geminiIssue, string type, Dictionary<string, JiraUser> userDictionary)
        {
            
            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = "ER",          //TODO issue.Project.Code,
                Summary = geminiIssue.Title,
                Description = ParseCommentEngine.Execute(geminiIssue.Description) + DateTime.Now.ToString(),    //TODO recueprare le immagini se presenti?
                Reporter = "70121:67b933a3-5693-47d2-82c0-3f997f279387", //TODO prendere dall'array degli accountID , partendo da geminiIssue.Reporter,
                
                //TODO status
                //Priority = geminiIssue.Priority,
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h",
                RemainingEstimate = geminiIssue.RemainingTime,
                //TODO
                DueDate = DateTime.MinValue,
                Resolution = geminiIssue.Resolution
            };
            


            //TODO jiraIssue.FixVersions


            //TODO con campo temporaneo e regola post creazione
            jiraIssue.Assignee = geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname;

            //Load all issue's attachment
            jiraIssue.Attachments = new List<string>();
            AttachmentGetter.Execute(jiraIssue, geminiIssue.Attachments);

            //Load and map all gemini comments
            CommentMapper.Execute(jiraIssue, geminiIssue, userDictionary);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue);

            //TODO Components 
            var functionality = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == FUNCTIONALITY);
            if (functionality != null && functionality.FormattedData != "")
                jiraIssue.Components.Add("BSM"); //TODO manca ERMAS jiraIssue.Components.Add(functionality.FormattedData);

            //TODO geminiIssue.Visibility

            //TODO Related Dev
            jiraIssue.RelatedDevelopment = GetRelatedDevelopment(geminiIssue);

            return jiraIssue;
        }


        #region Private        

        private string GetRelatedDevelopment(IssueDto geminiIssue)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == RELATED_DEVELOPMENT);
            if (relatedDev != null)
                return relatedDev.FormattedData;
            else
                return "";
        }

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            //UAT Type
            jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Type", "Defect"));   //TODO geminiIssue.Type : fare il mapping;

            //UAT Category
            if (geminiIssue.Components != null && geminiIssue.Components.Count > 0)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Category", geminiIssue.Components[0].Entity.Name));

            //UAT Severity
            jiraIssue.CustomFields.Add(new CustomFieldInfo("UAT Severity", geminiIssue.Severity));
            
            //UAT 
            if(geminiIssue.FixedInVersion != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Fixed In Build", geminiIssue.FixedInVersion));

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
