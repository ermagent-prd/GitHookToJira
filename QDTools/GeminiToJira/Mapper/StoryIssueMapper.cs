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
    public class StoryIssueMapper
    {
        private const string AFFECTEDBUILD = "AffectedBuild";
        private const string RELEASE_KEY = "Release Version";
        private const string LINE_KEY = "DVL";

        private readonly AttachmentGetter attachmentGetter;
        private readonly CommentMapper commentMapper;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly ParseCommentEngine parseCommentEngine;
        private readonly TimeLogEngine timeLogEngine;
        private readonly Dictionary<string, string> STATUS_MAPPING = new Dictionary<string, string>()
        {
            { "backlog",        "Backlog" },
            { "in Backlog",     "Backlog" },
            { "assigned",       "Select for development" },
            { "analysis",       "In Progress" },
            { "development",    "In progress" },
            { "waiting for test", "In progress" },
            { "testing", "In progress" },
            { "cancelled", "Done" },
            { "done", "Done" },
            { "in progress", "In progress" },
        };
        private readonly string STATUS_MAPPING_DEFAULT = "Backlog";

        private readonly Dictionary<string, string> TASK_TYPE_MAPPING = new Dictionary<string, string>()
        {
            { "analysis", "Analysis" },
            { "development", "Development" },
            { "test",       "Test" },
            { "documentation",    "Documentation" },
        };
        private readonly string TASK_TYPE_MAPPING_DEFAULT = "Other";

        private readonly Dictionary<string, string> ESTIMATE_TYPE_MAPPING = new Dictionary<string, string>()
        {
            { "Planned", "Planned" },
            { "CR-mkt", "Internal change request" },
            { "CR-int",  "Market change request" },
            { "Overbudget", "Overbudget" },
        };

        private readonly string ESTIMATE_TYPE_MAPPING_DEFAULT = "Planned";

        public StoryIssueMapper(
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

            string status = "";
            if (STATUS_MAPPING.TryGetValue(geminiIssue.Status.ToLower(), out status))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", status));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("StatusTmp", STATUS_MAPPING_DEFAULT));

            //AffectedBuild
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == AFFECTEDBUILD);
            if (affectedBuild != null && affectedBuild.FormattedData != "")
                jiraIssue.AffectVersions.Add(affectedBuild.FormattedData);

            //Assignee
            var owner = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Owner");
            if (owner != null && owner.FormattedData != "")
                jiraIssue.Assignee = accountEngine.Execute(owner.FormattedData).AccountId;

            //Load all issue's attachment
            jiraIssue.Attachments = descAttachments;
            attachmentGetter.Execute(jiraIssue, geminiIssue.Attachments, configurationSetup.Gemini.ProjectUrl, configurationSetup.AttachmentDownloadedPath);

            //Load and map all gemini comments
            commentMapper.Execute(configurationSetup, jiraIssue, geminiIssue);

            if (type == configurationSetup.Jira.StoryTypeCode)
            {
                LoadStoryCustomFields(jiraIssue, geminiIssue);

                //FixVersion
                var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == RELEASE_KEY);
                if (release != null && release.FormattedData != "")
                    jiraIssue.FixVersions.Add(release.FormattedData);

                SetComponents(geminiIssue, jiraIssue, configurationSetup.ComponentsForDevelopment);
            }
            else
                LoadStorySubTaskCustomFields(jiraIssue, geminiIssue);

            //Load custom fields in common
            LoadCommonCustomFields(jiraIssue, geminiIssue, configurationSetup.Gemini.ErmPrefix);

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
                jiraIssue.Logged = timeLogEngine.Execute(geminiIssue.TimeEntries);
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
    
        private static void SetComponents(IssueDto geminiIssue, CreateIssueInfo jiraIssue, List<string> components)
        {
            var devLine = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == LINE_KEY);
            if (devLine != null && devLine.Entity.Data != "")
            {
                foreach (var component in components)
                {
                    if (devLine.Entity.Data.Contains(component))
                        jiraIssue.Components.Add(devLine.Entity.Data);
                }
            }
        }


        private void LoadCommonCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue, string ermPrefix)
        {
            //Estimate Type
            var estimateType = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Estimate Type");
            if (estimateType != null && estimateType.FormattedData != "" && ESTIMATE_TYPE_MAPPING.TryGetValue(estimateType.FormattedData, out string jiraEstimateType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Estimate Type", jiraEstimateType));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Estimate Type", ESTIMATE_TYPE_MAPPING_DEFAULT));

            //Start Date
            if (geminiIssue.StartDate.HasValue)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Start date", geminiIssue.StartDate.Value.ToString("yyyy-M-d")));  //US Format

            //Gemini : save the original issue's code from gemini
            jiraIssue.CustomFields.Add(new CustomFieldInfo("Gemini", ermPrefix + geminiIssue.Id.ToString()));                        
        }

        private void LoadStorySubTaskCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            var taskType = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "TaskType");
            if (taskType != null && taskType.FormattedData != "" && TASK_TYPE_MAPPING.TryGetValue(taskType.FormattedData, out string jiraTaskType))
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Task Type", jiraTaskType));
            else
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Task Type", TASK_TYPE_MAPPING_DEFAULT));

            var refAnalysis = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "RefAnalysis");
            if (refAnalysis != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Documentation Owners", accountEngine.Execute(refAnalysis.FormattedData).AccountId));

            //"Changes Document"
            var analysisUrl = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Analysis Links");
            if (analysisUrl != null && analysisUrl.FormattedData != "" && !analysisUrl.FormattedData.Contains("\n") && !analysisUrl.FormattedData.Contains("\r"))
                jiraIssue.ChangeDocumentUrl = analysisUrl.FormattedData;
        }

        private void LoadStoryCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            //JDE Code
            var jdeCode = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "JDE");
            if (jdeCode != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("JDE", jdeCode.FormattedData));

            var refAnalysis = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "RefAnalysis");
            if (refAnalysis != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Analysis Owner", accountEngine.Execute(refAnalysis.FormattedData).AccountId));

            //BR Analysis Url
            var brAnalysisUrl = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Requirements");
            if (brAnalysisUrl != null && brAnalysisUrl.FormattedData != "" && !brAnalysisUrl.FormattedData.Contains("\n") && !brAnalysisUrl.FormattedData.Contains("\r"))
                jiraIssue.BrAnalysisUrl = brAnalysisUrl.FormattedData;

            //Security Activities
            var securityActivities = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Securirty Activities");
            if (securityActivities != null && securityActivities.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Security Activities", securityActivities.FormattedData));

            //GDPR activities
            var gdprActivities = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "GDPR Activities");
            if (gdprActivities != null && gdprActivities.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("GDPR Activities", gdprActivities.FormattedData));

            //IT Responsible 
            var itResponsible = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "RefIT");
            if (itResponsible != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("IT Responsible", accountEngine.Execute(itResponsible.FormattedData).AccountId));

            var testResponsible = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "RefTest");
            if (testResponsible != null)
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Test Responsible", accountEngine.Execute(testResponsible.FormattedData).AccountId));

            //Analysis start date
            var analysisStartDate = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "AnalysisStartDate");
            if (analysisStartDate != null && analysisStartDate.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Analysis Start Date", InternalConvertDate(analysisStartDate.FormattedData).ToString("yyyy-M-d")));

            //Test start date
            var testStartDate = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Test Start Date");
            if (testStartDate != null && testStartDate.FormattedData != "")
                jiraIssue.CustomFields.Add(new CustomFieldInfo("Test Start Date", InternalConvertDate(testStartDate.FormattedData).ToString("yyyy-M-d")));

            //"Test Document"
            var testDocumentUrl = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Test Document");
            if (testDocumentUrl != null && testDocumentUrl.FormattedData != "" && !testDocumentUrl.FormattedData.Contains("\n") && !testDocumentUrl.FormattedData.Contains("\r"))
                jiraIssue.TestDocumentUrl = testDocumentUrl.FormattedData;

            //"Changes Document"
            var changeDocumentUrl = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "EDEVChangesDoc");
            if (changeDocumentUrl != null && changeDocumentUrl.FormattedData != "" && !changeDocumentUrl.FormattedData.Contains("\n") && !changeDocumentUrl.FormattedData.Contains("\r"))
                jiraIssue.ChangeDocumentUrl = changeDocumentUrl.FormattedData;

            //New Feature Doc Url
            var newFeatureDocumentUrl = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "New feature slide");
            if (newFeatureDocumentUrl != null && newFeatureDocumentUrl.FormattedData != "" && !newFeatureDocumentUrl.FormattedData.Contains("\n") && !newFeatureDocumentUrl.FormattedData.Contains("\r"))
                jiraIssue.NewFeatureDocumentUrl = newFeatureDocumentUrl.FormattedData;

            //"Analysis Links"
            var analysisUrl = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Analysis Links");
            if (analysisUrl != null && analysisUrl.FormattedData != "" && !analysisUrl.FormattedData.Contains("\n") && !analysisUrl.FormattedData.Contains("\r"))
                jiraIssue.AnalysisUrl = analysisUrl.FormattedData;
        }

        #endregion

        private DateTime InternalConvertDate(String dateValue)
        {
            double dt;
            DateTime dtDate;

            if (DateTime.TryParse(dateValue, out dtDate))
            {
                return dtDate.Date;
            }
            else
            {
                String[] formats = new String[4] { "dd/MM/yyyy", "MM/dd/yyyy", "dd/MM/yy", "MM/dd/yy" };

                if (DateTime.TryParseExact(dateValue, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dtDate))
                    return dtDate.Date;                
            }

            return DateTime.Now;
        }
    }
}
