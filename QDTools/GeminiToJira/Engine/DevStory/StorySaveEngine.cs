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
            int? geminiIssueId,
            string reporter, 
            CreateIssueInfo jiraIssueInfo, 
            GeminiToJiraParameters configurationSetup)
        {
            //save story
            var jiraIssue = this.jiraSaveEngine.Execute(
                jiraIssueInfo,
                configurationSetup.Jira,
                configurationSetup.AttachmentDownloadedPath);
            //and set as saved
            if (geminiIssueId.HasValue && !jiraSavedDictionary.TryGetValue(geminiIssueId.Value, out Issue existing))
                jiraSavedDictionary.Add(geminiIssueId.Value, jiraIssue);

            //save reporter
            this.reporterEngine.Execute(jiraIssue, reporter, configurationSetup.Jira.DefaultAccount);

            return jiraIssue;
        }


        #endregion

        #region Private methods


        #endregion
    }
}
