using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine.Common.Alfresco;
using GeminiToJira.Engine.DevStory;
using GeminiToJira.Log;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters.Import;

namespace GeminiToJira.Engine
{
    public class ImportStoryEngine
    {
        private readonly StoryIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly FilteredGeminiIssueListGetter issueGetter;
        private readonly LogManager logManager;
        private readonly AlfrescoUrlsEngine alfrescoEngine;
        private readonly StorySaveEngine storySaveEngine;
        private readonly SubtaskSaveEngine subTaskSaveEngine;
        private readonly ImportStoryGroupItemEngine importStoryGroupEngine;
        private readonly ImportStoryNoGroupEngine noGroupEngine;


        public ImportStoryEngine(
            StoryIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            FilteredGeminiIssueListGetter issueGetter,
            LogManager logManager,
            AlfrescoUrlsEngine alfrescoEngine,
            StorySaveEngine storySaveEngine,
            SubtaskSaveEngine subTaskSaveEngine,
            ImportStoryGroupItemEngine importStoryGroupEngine,
            ImportStoryNoGroupEngine noGroupEngine)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.issueGetter = issueGetter;
            this.logManager = logManager;
            this.alfrescoEngine = alfrescoEngine;
            this.storySaveEngine = storySaveEngine;
            this.subTaskSaveEngine = subTaskSaveEngine;
            this.importStoryGroupEngine = importStoryGroupEngine;
            this.noGroupEngine = noGroupEngine;
        }

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            var projectCode = configurationSetup.JiraProjectCode;

            var jiraSavedDictionary = new Dictionary<int, Issue>();
            var storyFolderDictionary = new Dictionary<string, string>();

            var storyType = configurationSetup.Jira.StoryTypeCode;
            var storySubTaskType = configurationSetup.Jira.SubTaskTypeCode;

            var geminiDevelopmentIssueList = this.issueGetter.Execute(configurationSetup);

            
            var storyLogFile = "DevelopmentLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            this.logManager.SetLogFile(configurationSetup.LogDirectory + storyLogFile);

            var filteredDevelopments = 
                geminiDevelopmentIssueList.Where(l => l.Type == "Development" || l.Type == "Enhancement")
                .OrderBy(f => f.Id).ToList();

            foreach (var geminiIssue in filteredDevelopments)
            {
                if (!filterIssue(configurationSetup,geminiIssue))
                    continue;
                
                //Story with Group
                var importDone = this.importStoryGroupEngine.Execute(
                        configurationSetup,
                        projectCode,
                        jiraSavedDictionary,
                        storyFolderDictionary,
                        storyType,
                        storySubTaskType,
                        filteredDevelopments,
                        geminiIssue);

                if (importDone)
                    continue;

                this.noGroupEngine.Execute(
                    configurationSetup, 
                    projectCode, 
                    jiraSavedDictionary, 
                    storyFolderDictionary, 
                    storyType, 
                    storySubTaskType, 
                    filteredDevelopments, 
                    geminiIssue);
                
            }

            //orphansManagement(configurationSetup, projectCode, jiraSavedDictionary, storyFolderDictionary, storyType, storySubTaskType, geminiDevelopmentIssueList, storyLogFile);
        }

        #region Private 

        private bool filterIssue(GeminiToJiraParameters configurationSetup, IssueDto issue)
        {
            if (configurationSetup.Filter.SKIPPED_ISSUES != null &&
                configurationSetup.Filter.SKIPPED_ISSUES.Contains(issue.IssueKey))
                return false;

            if (configurationSetup.Filter.INVALID_STATUSES != null && 
                configurationSetup.Filter.INVALID_STATUSES.Contains(issue.Status))
                return false;

            if (configurationSetup.Filter.SELECTED_ISSUES != null &&
                configurationSetup.Filter.SELECTED_ISSUES.Contains(issue.IssueKey))
                return true;

            return false;
        }


        private void orphansManagement(GeminiToJiraParameters configurationSetup, string projectCode, Dictionary<int, Issue> jiraSavedDictionary, Dictionary<string, string> storyFolderDictionary, string storyType, string storySubTaskType, IEnumerable<IssueDto> geminiDevelopmentIssueList, string storyLogFile)
        {
            #region Orphans
            //manage orphans if exist (an orphan is a gemini task that was not listed in the hierarhy of a Story, that was previous inserted
            foreach (var geminiIssue in geminiDevelopmentIssueList.Where(l => l.Type == "Task").OrderBy(f => f.Id).ToList())
            {
                if (!jiraSavedDictionary.TryGetValue(geminiIssue.Id, out Issue jiraIssueSaved))
                {
                    var currentSubIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                    try
                    {
                        //if have no father
                        if (currentSubIssue.HierarchyKey == "")
                        {
                            //Create Story
                            var jiraIssueInfo = geminiToJiraMapper.Execute(configurationSetup, currentSubIssue, storyType, projectCode);
                            Issue jiraIssue = this.storySaveEngine.Execute(
                                jiraSavedDictionary,
                                geminiIssue.Id,
                                geminiIssue.Reporter,
                                jiraIssueInfo, 
                                configurationSetup);

                            var storyFolder = this.alfrescoEngine.Execute(jiraIssueInfo, jiraIssue, "", configurationSetup);

                            storyFolderDictionary.Add(jiraIssue.JiraIdentifier, storyFolder);

                            //and become a subtask of myself
                            this.subTaskSaveEngine.Execute(configurationSetup, jiraIssue, currentSubIssue, jiraSavedDictionary, storySubTaskType, storyFolder, checkRelease: true);

                        }
                        else
                        {
                            //my father was inserted and i was not in his hierarchy list
                            var fatherKey = currentSubIssue.HierarchyKey.Split('|').ElementAt(1);

                            if (jiraSavedDictionary.TryGetValue(Convert.ToInt32(fatherKey), out Issue jiraFatherIssue))
                            {
                                storyFolderDictionary.TryGetValue(jiraFatherIssue.JiraIdentifier, out string storyFolder);

                                this.subTaskSaveEngine.Execute(configurationSetup, jiraFatherIssue, currentSubIssue, jiraSavedDictionary, storySubTaskType, storyFolder, checkRelease: true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logManager.Execute("[SubStory] - " + currentSubIssue.IssueKey + " - " + ex.Message);
                    }
                }
            }

            #endregion
        }






        #endregion
    }


}
