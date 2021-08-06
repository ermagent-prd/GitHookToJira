using System.Collections.Generic;
using System.Linq;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiToJira.Engine.Common;
using GeminiToJira.Parameters.Import;
using GeminiTools.Items;
using JiraTools.Model;
using static GeminiToJira.Engine.JiraAccountIdEngine;

namespace GeminiToJira.Mapper
{
    public class StoryGroupIssueMapper
    {
        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;
        private readonly TimeLogEngine timeLogEngine;
        private readonly AddCustomFieldEngine customeFieldEngine;
        private readonly AddWatchersEngine watchersEngine;
        private readonly URLChecker urlChecker;


        public StoryGroupIssueMapper(
            CommentMapper commentMapper, 
            AttachmentGetter attachmentGetter,
            JiraAccountIdEngine accountEngine,
            ParseCommentEngine parseCommentEngine,
            TimeLogEngine timeLogEngine,
            AddCustomFieldEngine customeFieldEngine,
            AddWatchersEngine watchersEngine,
            URLChecker urlChecker)
        {
            this.attachmentGetter = attachmentGetter;
            this.commentMapper = commentMapper;
            this.accountEngine = accountEngine;
            this.parseCommentEngine = parseCommentEngine;
            this.timeLogEngine = timeLogEngine;
            this.customeFieldEngine = customeFieldEngine;
            this.watchersEngine = watchersEngine;
            this.urlChecker = urlChecker;
        }

        public CreateIssueInfo Execute(
            GeminiToJiraParameters configurationSetup, 
            IssueDto geminiGroupIssue, 
            string type, 
            string projectCode,
            IssueDto geminiDevIssue)
        {
            var descAttachments = new List<string>();

            var jiraIssue = new CreateIssueInfo
            {
                ProjectKey = projectCode,
                Summary = geminiGroupIssue.Title.TrimEnd(),
                Description = parseCommentEngine.Execute(geminiGroupIssue.Description, "desc", descAttachments, configurationSetup.AttachmentDownloadedPath),
                Type = type,
            };

            jiraIssue.Priority = geminiGroupIssue.Priority;

            jiraIssue.OriginalEstimate = geminiGroupIssue.EstimatedHours + "h " + geminiGroupIssue.EstimatedMinutes + "m";
            CalculateRemainigEstimate(configurationSetup, geminiGroupIssue, type, jiraIssue);

            jiraIssue.AffectVersions = new List<string>();
            jiraIssue.FixVersions = new List<string>();

            if (geminiGroupIssue.DueDate.HasValue)
                jiraIssue.DueDate = geminiGroupIssue.DueDate.Value;            

            //AffectedBuild
            var affectedBuild = geminiGroupIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Mapping.AFFECTEDBUILD_LABEL);
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.AffectVersions.Add(affectedBuild.FormattedData);

            //Assignee
            var owner = geminiGroupIssue.CustomFields.FirstOrDefault(i => i.Name == "Owner");
            if (owner != null && owner.FormattedData != "")
                jiraIssue.Assignee = accountEngine.Execute(owner.FormattedData, configurationSetup.Jira.DefaultAccount).AccountId;

            //Load all issue's attachment
            jiraIssue.Attachments = descAttachments;
            this.attachmentGetter.Execute(jiraIssue, geminiGroupIssue.Attachments, configurationSetup.Gemini.ProjectUrl, configurationSetup.AttachmentDownloadedPath);

            //Load and map all gemini comments
            commentMapper.Execute(configurationSetup, jiraIssue, geminiGroupIssue);

            if (type == configurationSetup.Jira.StoryTypeCode)
            {
                LoadStoryCustomFields(
                    jiraIssue, 
                    geminiGroupIssue,
                    geminiDevIssue,
                    configurationSetup.Mapping);


                //FixVersion
                var release = geminiGroupIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Mapping.RELEASE_KEY_LABEL);
                if (release != null && release.FormattedData != "")
                    jiraIssue.FixVersions.Add(release.FormattedData);

                //Development Line
                SetDevLine(geminiGroupIssue, jiraIssue, configurationSetup.Mapping.DEV_LINE_MAPPING, configurationSetup.Mapping);

            }
            else
                LoadStorySubTaskCustomFields(jiraIssue, geminiGroupIssue, configurationSetup.Mapping);

            //Watchers
            this.watchersEngine.Execute(jiraIssue, geminiGroupIssue);

            //Load custom fields in common
            LoadCommonCustomFields(jiraIssue, geminiGroupIssue, configurationSetup.Mapping, configurationSetup.Gemini.ErmPrefix);

            return jiraIssue;
        }


        #region Private    



        private void CalculateRemainigEstimate(GeminiToJiraParameters configurationSetup, IssueDto geminiIssue, string type, CreateIssueInfo jiraIssue)
        {
            if (type == configurationSetup.Jira.StoryTypeCode)
            {
                //story
                jiraIssue.RemainingEstimate = geminiIssue.RemainingTime;
            }
            else
            {
                //story-subtask
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
                jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries, configurationSetup.Jira.DefaultAccount);
            }
        }
  
        private int GetTotalTimeEntries(List<IssueTimeTrackingDto> timeEntries)
        {
            int minuteSpent = 0;
            foreach (var timeEntry in timeEntries)
            {
                
                minuteSpent += (timeEntry.Entity.Hours*60) + timeEntry.Entity.Minutes;
            }

            return minuteSpent;
        }
    
        private static void SetDevLine(IssueDto geminiIssue, CreateIssueInfo jiraIssue, Dictionary<string,string> devLinesDict, JiraTools.Parameters.MappingConfiguration mapping)
        {
            if (devLinesDict == null || !devLinesDict.Any())
                return;

            var devLine = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.LINE_KEY_LABEL);
            if (devLine != null && devLine.Entity.Data != "")
            {
                string devLineValue;
                if (devLinesDict.TryGetValue(devLine.Entity.Data, out devLineValue))
                    jiraIssue.CustomFields.Add(new CustomFieldInfo("Development Line", devLineValue));
            }
        }


        private void LoadCommonCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, JiraTools.Parameters.MappingConfiguration mapping, string ermPrefix)
        {
            string status = "";
            if (mapping.DEV_STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", mapping.DEV_STATUS_MAPPING_DEFAULT));

            //Start Date
            if (geminiIssue.StartDate.HasValue)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Start date", geminiIssue.StartDate.Value.ToString("yyyy-M-d")));  //US Format

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("OriginalKey", ermPrefix + geminiIssue.Id.ToString()));                        
        }

        private void LoadStorySubTaskCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, JiraTools.Parameters.MappingConfiguration mapping)
        {
            var taskType = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "TaskType");
            if (taskType != null && taskType.FormattedData != "" && mapping.TASK_TYPE_MAPPING.TryGetValue(taskType.FormattedData, out string jiraTaskType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Task Type", jiraTaskType));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Task Type", mapping.TASK_TYPE_MAPPING_DEFAULT));
        }

        private void LoadStoryCustomFields(
            CreateIssueInfo jiraIssue, 
            IssueDto geminiGroupIssue,
            IssueDto geminiDevIssue,
            JiraTools.Parameters.MappingConfiguration mapping)
        {
            //Estimate Type
            var estimateType = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "Estimate Type");
            if (estimateType != null && estimateType.FormattedData != "" && mapping.DEV_ESTIMATE_TYPE_MAPPING.TryGetValue(estimateType.FormattedData.ToLower(), out string jiraEstimateType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Estimate Type", jiraEstimateType));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Estimate Type", mapping.DEV_ESTIMATE_TYPE_MAPPING_DEFAULT));

            //JDE Code
            var jdeCode = geminiGroupIssue.CustomFields.FirstOrDefault(i => i.Name == "JDE");
            if (jdeCode != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("JDE Module", jdeCode.FormattedData));

            //BR Analysis Url
            var brAnalysisUrl = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "Requirements");
            if (brAnalysisUrl != null && brAnalysisUrl.FormattedData != "" && !brAnalysisUrl.FormattedData.Contains("\n") && !brAnalysisUrl.FormattedData.Contains("\r"))
                jiraIssue.BrAnalysisUrl = brAnalysisUrl.FormattedData;

            //Security Activities
            var securityActivities = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "Securirty Activities");
            if (securityActivities != null && securityActivities.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Security Activities", securityActivities.FormattedData));

            //GDPR activities
            var gdprActivities = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "GDPR Activities");
            if (gdprActivities != null && gdprActivities.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("GDPR Activities", gdprActivities.FormattedData));

            //"Test Document"
            var testDocumentUrl = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "Test Document");
            if (testDocumentUrl != null && testDocumentUrl.FormattedData != "" && !testDocumentUrl.FormattedData.Contains("\n") && !testDocumentUrl.FormattedData.Contains("\r"))
                jiraIssue.TestDocumentUrl = testDocumentUrl.FormattedData;

            //"Changes Document"
            var changeDocumentUrl = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "EDEVChangesDoc");
            if (changeDocumentUrl != null && changeDocumentUrl.FormattedData != "" && !changeDocumentUrl.FormattedData.Contains("\n") && !changeDocumentUrl.FormattedData.Contains("\r"))
                jiraIssue.ChangeDocumentUrl = changeDocumentUrl.FormattedData;

            //New Feature Doc Url
            var newFeatureDocumentUrl = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "New Features Slide");
            if (newFeatureDocumentUrl != null && newFeatureDocumentUrl.FormattedData != "" && !newFeatureDocumentUrl.FormattedData.Contains("\n") && !newFeatureDocumentUrl.FormattedData.Contains("\r"))
                jiraIssue.NewFeatureDocumentUrl = newFeatureDocumentUrl.FormattedData;

            //"Analysis Links"
            var analysisUrl = geminiDevIssue.CustomFields.FirstOrDefault(i => i.Name == "Analysis Links");
            if (!string.IsNullOrWhiteSpace(analysisUrl?.FormattedData) && !analysisUrl.FormattedData.Contains("\n") && !analysisUrl.FormattedData.Contains("\r"))
                jiraIssue.AnalysisUrl = this.urlChecker.Execute(analysisUrl.FormattedData);
        }

        #endregion

        
    }
}
