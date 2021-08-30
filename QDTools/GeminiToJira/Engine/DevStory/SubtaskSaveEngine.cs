using System.Collections.Generic;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine.Common;
using GeminiToJira.Engine.Common.Alfresco;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters.Import;
using JiraTools.Engine;
using JiraTools.Model;

namespace GeminiToJira.Engine.DevStory
{
    public class SubtaskSaveEngine
    {
        #region Private properties

        private readonly CreateIssueEngine jiraSaveEngine;

        private readonly ReporterSaveEngine reporterEngine;

        private readonly GeminiIssueChecker issueChecker;

        private readonly AlfrescoUrlsEngine alfrescoEngine;

        private readonly StoryIssueMapper geminiToJiraMapper;

        #endregion

        #region Constructor

        public SubtaskSaveEngine(
            CreateIssueEngine jiraSaveEngine, 
            ReporterSaveEngine reporterEngine, 
            GeminiIssueChecker issueChecker, 
            AlfrescoUrlsEngine alfrescoEngine, 
            StoryIssueMapper geminiToJiraMapper)
        {
            this.jiraSaveEngine = jiraSaveEngine;
            this.reporterEngine = reporterEngine;
            this.issueChecker = issueChecker;
            this.alfrescoEngine = alfrescoEngine;
            this.geminiToJiraMapper = geminiToJiraMapper;
        }

        
        #endregion



        #region Public methods

        public void Execute(
           GeminiToJiraParameters configurationSetup,
           Issue jiraIssue,
           IssueDto currentSubIssue,
           Dictionary<int, Issue> jiraSavedDictionary,
           string storySubTaskType,
           string rooStoryFolder,
           bool checkRelease)
        {
            if (checkRelease && !this.issueChecker.Execute(currentSubIssue, configurationSetup))
                return;

            var jiraStorySubTaskInfo = this.geminiToJiraMapper.Execute(configurationSetup, currentSubIssue, storySubTaskType, configurationSetup.JiraProjectCode);

            jiraStorySubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;

            //create subtask
            var subIssue = jiraSaveEngine.Execute(
                jiraStorySubTaskInfo,
                configurationSetup.Jira,
                configurationSetup.AttachmentDownloadedPath);
            //and set as saved
            if (!jiraSavedDictionary.TryGetValue(currentSubIssue.Id, out Issue existing))
                jiraSavedDictionary.Add(currentSubIssue.Id, subIssue);

            this.reporterEngine.Execute(subIssue, currentSubIssue.Reporter, configurationSetup.Jira.DefaultAccount);

            this.alfrescoEngine.Execute(jiraStorySubTaskInfo, subIssue, rooStoryFolder, configurationSetup);
        }



        #endregion

        #region Private methods


        #endregion
    }
}
