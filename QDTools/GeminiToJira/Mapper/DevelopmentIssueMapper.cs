using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Items;
using JiraTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiToJira.Mapper
{
    public class DevelopmentIssueMapper
    {
        private const string AFFECTEDBUILD = "AffectedBuild";
        private const string DEVELOPMENT_RELEASE_KEY = "Release Version";

        public DevelopmentIssueMapper()
        {
        }

        public CreateIssueInfo Execute(IssueDto geminiIssue, string type, Dictionary<string, JiraUser> userDictionary)
        {
            
            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = "ER",          //TODO issue.Project.Code,
                Summary = geminiIssue.Title,
                Description = geminiIssue.Description + DateTime.Now.ToString(),
                //Priority = geminiIssue.Priority,
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h",
                RemainingEstimate = geminiIssue.RemainingTime,
                DueDate = new DateTime(2021, 12, 31),   //TODO
                //TODO Reporter = "70121:67b933a3-5693-47d2-82c0-3f997f279387" //TODO prendere dall'array degli accountID , partendo da geminiIssue.Reporter,
            };

            //AffectedBuild
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == AFFECTEDBUILD);
            if(affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.FixVersions.Add(affectedBuild.FormattedData);

            //FixVersion
            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
            if (release != null && release.FormattedData != "")
                jiraIssue.FixVersions.Add(release.FormattedData);

            //TODO ?? var devLine = issue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_LINE_KEY);
            //TODO ?? if (devLine != null && devLine.FormattedData != "")
            //TODO ??     mappedIssue.FixVersions.Add(devLine.FormattedData);

            //TODO
            jiraIssue.Assignee = geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname;

            //Load all issue's attachment
            jiraIssue.Attachments = new List<string>();
            AttachmentGetter.Execute(jiraIssue, geminiIssue.Attachments);

            //Load and map all gemini comments
            CommentMapper.Execute(jiraIssue, geminiIssue, userDictionary);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue);

            //TODO Components --> sono solo per le UAT?
            //issueInfo.Components.Add("ILIAS");

            //TODO 
            //mappedIssue.Logged.Add(new WorkLogInfo(
            //    "Pierluigi Nanni",
            //    DateTime.Now,
            //    "1d",
            //    "WorkLog Logging test"));

            return jiraIssue;
        }


        #region Private        

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            //TODO da caricare solo quelli creati in JIRA 
            //foreach (var field in issue.CustomFields)
            //    jiraIssue.CustomFields.Add(new CustomFieldInfo(field.Name, field.FormattedData));

            //TODO
            //Reporter
            //il valore da salvare è l'accountId, recuperato dal dizionario degli utenti
            jiraIssue.CustomFields.Add(new CustomFieldInfo("OwnerTmp", geminiIssue.Creator));

            //Assignee
            //TODO: da stituire ResourcesTmp con AssigneeTmp, il quale dovrà essere puntato dalal regola su JIRA
            //il valore da salvare è l'accountId, recuperato dal dizionario degli utenti
            jiraIssue.CustomFields.Add(new CustomFieldInfo("ResourcesTmp", geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname));

        }

        #endregion
    }
}
