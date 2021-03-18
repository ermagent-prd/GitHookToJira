using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiToJira.Parameters;
using GeminiToJira.Parameters.Import;
using GeminiTools.Engine;
using GeminiTools.Items;
using GeminiTools.Parameters;
using JiraTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiToJira.Mapper
{
    public class TaskIssueMapper
    {
        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;
        private readonly TimeLogEngine timeLogEngine;

        public TaskIssueMapper(
            CommentMapper commentMapper, 
            AttachmentGetter attachmentGetter,
            JiraAccountIdEngine accountEngine,
            ParseCommentEngine parseCommentEngine,
            LinkItemEngine linkItemEngine,
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
            };

            jiraIssue.Priority = geminiIssue.Priority;
            jiraIssue.OriginalEstimate = geminiIssue.EstimatedHours + "h " + geminiIssue.EstimatedMinutes + "m";

            CalculateRemainigEstimate(configurationSetup, geminiIssue, type, jiraIssue);

            jiraIssue.AffectVersions = new List<string>();
            jiraIssue.FixVersions = new List<string>();

            if (geminiIssue.DueDate.HasValue)
                jiraIssue.DueDate = geminiIssue.DueDate.Value;


            //Assignee
            var owner = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Owner");
            if (owner != null && owner.FormattedData != "")
                jiraIssue.Assignee = accountEngine.Execute(owner.FormattedData).AccountId;

            //Load all issue's attachment
            jiraIssue.Attachments = descAttachments;
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments, configurationSetup.Gemini.ProjectUrl, configurationSetup.AttachmentDownloadedPath);

            //Load and map all gemini comments
            commentMapper.Execute(configurationSetup, jiraIssue, geminiIssue);

            LoadTaskCustomFields(jiraIssue, geminiIssue, configurationSetup.Gemini.ErmPrefix, configurationSetup.Mapping);

            SetComponents(geminiIssue, jiraIssue, configurationSetup.ComponentsForDevelopment, configurationSetup.Mapping);

            //FixVersion
            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Mapping.RELEASE_KEY_LABEL);
            if (release != null && release.FormattedData != "")
                jiraIssue.FixVersions.Add(release.FormattedData);


            return jiraIssue;
        }


        #region Private       
        private void CalculateRemainigEstimate(GeminiToJiraParameters configurationSetup, IssueDto geminiIssue, string type, CreateIssueInfo jiraIssue)
        {
            if (type == configurationSetup.Jira.TaskTypeCode)
            {
                jiraIssue.RemainingEstimate = geminiIssue.RemainingTime;
            }
            else
            {
                //subtask
                if (geminiIssue.Status.ToLower() != "done")
                {
                    //total time entries in minutes
                    int loggedTime = GetTotalTimeEntries(geminiIssue.TimeEntries);
                    int estimatedTime = (geminiIssue.EstimatedHours * 60) + geminiIssue.EstimatedMinutes;
                   
                    if (estimatedTime < loggedTime && geminiIssue.PercentComplete > 0)
                    {
                        double percentage = (1 - ((double)geminiIssue.PercentComplete / 100));

                        //loggedTime = estimated + exceeded
                        jiraIssue.RemainingEstimate = (loggedTime + (percentage * estimatedTime)) + "m";
                    }
                }
                else
                    jiraIssue.RemainingEstimate = "0m";

                //worklog: only for sybtask issues
                jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries);
            }
        }


        
        private int GetTotalTimeEntries(List<IssueTimeTrackingDto> timeEntries)
        {
            int minuteSpent = 0;
            foreach (var timeEntry in timeEntries)
                minuteSpent += (timeEntry.Entity.Hours*60) + timeEntry.Entity.Minutes;

            return minuteSpent;
        }
    
        private static void SetComponents(IssueDto geminiIssue, CreateIssueInfo jiraIssue, List<string> components, JiraTools.Parameters.MappingConfiguration mapping)
        {
            var devLine = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.LINE_KEY_LABEL);
            if (devLine != null && devLine.Entity.Data != "")
            {
                foreach (var component in components)
                {
                    if (devLine.Entity.Data.Contains(component))
                        jiraIssue.Components.Add(devLine.Entity.Data);
                }
            }
        }        

        private void LoadTaskCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string ermPrefix, JiraTools.Parameters.MappingConfiguration mapping)
        {
            //Start Date
            if (geminiIssue.StartDate.HasValue)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Start date", geminiIssue.StartDate.Value.ToString("yyyy-M-d")));  //US Format

            var estimateType = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Estimate Type");
            if (estimateType != null && estimateType.FormattedData != "" && mapping.DEV_ESTIMATE_TYPE_MAPPING.TryGetValue(estimateType.FormattedData.ToLower(), out string jiraEstimateType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Estimate Type", jiraEstimateType));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Estimate Type", mapping.DEV_ESTIMATE_TYPE_MAPPING_DEFAULT));

            string status = "";
            if (mapping.DEV_STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", mapping.DEV_STATUS_MAPPING_DEFAULT));

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("Gemini", ermPrefix + geminiIssue.Id.ToString()));

            //JDE Code
            var jdeCode = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "JDE");
            if (jdeCode != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("JDE Module", jdeCode.FormattedData));
        }

        #endregion
    }
}
