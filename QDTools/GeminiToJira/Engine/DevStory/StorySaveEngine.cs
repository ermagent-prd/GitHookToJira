using System.Collections.Generic;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine.Common;
using GeminiToJira.Parameters.Import;
using JiraTools.Engine;
using JiraTools.Model;

namespace GeminiToJira.Engine.DevStory
{
    public class StorySaveEngine
    {
        #region Private properties

        private readonly CreateIssueEngine jiraSaveEngine;

        private readonly ReporterSaveEngine reporterEngine;

        #region Constructor
        public StorySaveEngine(CreateIssueEngine jiraSaveEngine, ReporterSaveEngine reporterEngine)
        {
            this.jiraSaveEngine = jiraSaveEngine;
            this.reporterEngine = reporterEngine;
        }

        #endregion

     

        #endregion

        #region Public methods

        public Issue Execute(
            Dictionary<int, Issue> jiraSavedDictionary, 
            IssueDto geminiIssue, 
            CreateIssueInfo jiraIssueInfo, 
            GeminiToJiraParameters configurationSetup)
        {
            //save story
            var jiraIssue = this.jiraSaveEngine.Execute(
                jiraIssueInfo,
                configurationSetup.Jira,
                configurationSetup.AttachmentDownloadedPath);
            //and set as saved
            if (!jiraSavedDictionary.TryGetValue(geminiIssue.Id, out Issue existing))
                jiraSavedDictionary.Add(geminiIssue.Id, jiraIssue);

            //save reporter
            this.reporterEngine.Execute(jiraIssue, geminiIssue, configurationSetup.Jira.DefaultAccount);

            return jiraIssue;
        }


        #endregion

        #region Private methods


        #endregion
    }
}
