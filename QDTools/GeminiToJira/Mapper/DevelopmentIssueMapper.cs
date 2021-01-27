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
    public class DevelopmentIssueMapper
    {
        private const string AFFECTEDBUILD = "AffectedBuild";
        private const string DEVELOPMENT_RELEASE_KEY = "Release Version";
        private const string DEVELOPMENT_LINE_KEY = "DVL";

        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;

        public DevelopmentIssueMapper(
            CommentMapper commentMapper, 
            AttachmentGetter attachmentGetter,
            JiraAccountIdEngine accountEngine)
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
                Description = geminiIssue.Description + DateTime.Now.ToString(),
                //Priority = geminiIssue.Priority,
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h",
                RemainingEstimate = geminiIssue.RemainingTime,
                //TODO Reporter = "70121:67b933a3-5693-47d2-82c0-3f997f279387" //TODO prendere dall'array degli accountID , partendo da geminiIssue.Reporter,
            };

            if (geminiIssue.DueDate.HasValue)
                jiraIssue.DueDate = geminiIssue.DueDate.Value;

            //AffectedBuild
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == AFFECTEDBUILD);
            if(affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.FixVersions.Add(affectedBuild.FormattedData);

            //FixVersion
            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
            if (release != null && release.FormattedData != "")
                jiraIssue.FixVersions.Add(release.FormattedData);

            //TODO
            jiraIssue.Assignee = geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname;

            //Load all issue's attachment
            jiraIssue.Attachments = new List<string>();
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments);

            //Load and map all gemini comments
            commentMapper.Execute(jiraIssue, geminiIssue);

            //Load custom fields
            LoadCustomFields(jiraIssue, geminiIssue, type);
                        
            var devLine = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_LINE_KEY);
            if (devLine != null && devLine.Entity.Data != "")
                jiraIssue.Components.Add(devLine.Entity.Data);



            return jiraIssue;
        }


        #region Private        

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string devType)
        {
            
            //TODO
            //Reporter
            //il valore da salvare è l'accountId, recuperato dal dizionario degli utenti
            jiraIssue.CustomFields.Add(new CustomFieldInfo("OwnerTmp", geminiIssue.Creator));

            //Assignee
            //TODO: da stituire ResourcesTmp con AssigneeTmp, il quale dovrà essere puntato dalal regola su JIRA
            //il valore da salvare è l'accountId, recuperato dal dizionario degli utenti
            jiraIssue.CustomFields.Add(new CustomFieldInfo("ResourcesTmp", geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname));

            //Start Date
            if (geminiIssue.StartDate.HasValue)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Start date", geminiIssue.StartDate.Value.ToString()));  //TODO non funziona il formato --> americano?

            //JDE Code
            var jdeCode = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "JDE Code");
            if (jdeCode != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("JDE", jdeCode.FormattedData));

            //TODO Security Activities
            //Security Activities
            //"Securirty Activities"
            var securityActivities = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Security Activities");
            if (securityActivities != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Security Activities", securityActivities.FormattedData));

            //TODO GDPR activities
            //GDPR Activities
            //"GDPR Activities"
            var gdprActivities = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "GDPR Activities");
            if (gdprActivities != null && gdprActivities.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("GDPR Activities", gdprActivities.FormattedData));

            if (devType != JiraConstants.SubTaskType)
            {
                var refAnalysis = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "RefAnalysis");
                if (refAnalysis != null)
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Analysis Owner", accountEngine.Execute(refAnalysis.FormattedData)));

                var itResponsible = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "RefIT");
                if (itResponsible != null)
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("IT Responsible", accountEngine.Execute(itResponsible.FormattedData)));

                var testResponsible = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "RefTest");
                if (testResponsible != null)
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Test Responsible", accountEngine.Execute(testResponsible.FormattedData)));
            }

            //TODO Analysis start date
            //Analysis Start Date
            //"AnalysisStartDate"
            var analysisStartDate = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "AnalysisStartDate");
            if (analysisStartDate != null && analysisStartDate.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Analysis Start Date", analysisStartDate.FormattedData));


            //TODO test start date
            //Test Start Date
            //"Test Start Date"
            var testStartDate = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Test Start Date");
            if (testStartDate != null && testStartDate.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Test Start Date", testStartDate.FormattedData));

            //TODO BR Analysis Url
            //BR Analysis Url
            //"Requirements"
            var brAnalysisUrl = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Requirements");
            if (brAnalysisUrl != null && brAnalysisUrl.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("BR Analysis Url", brAnalysisUrl.FormattedData));

            //TODO Gemini --> da salvare per il reperimento poi dei development collegati per le UAT
            //Gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("Gemini", GeminiConstants.ErmPrefix + geminiIssue.Id.ToString()));

        }

        #endregion
    }
}
