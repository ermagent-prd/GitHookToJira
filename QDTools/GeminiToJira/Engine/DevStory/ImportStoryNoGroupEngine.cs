using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine.Common.Alfresco;
using GeminiToJira.Log;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters.Import;
using GeminiTools.Items;
using JiraTools.Model;

namespace GeminiToJira.Engine.DevStory
{
    public class ImportStoryNoGroupEngine
    {
        #region Private properties

        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;

        private readonly StoryIssueMapper geminiToJiraMapper;

        private readonly StorySaveEngine storySaveEngine;

        private readonly AlfrescoUrlsEngine alfrescoEngine;

        private readonly SubtaskSaveEngine subTaskSaveEngine;

        private readonly LogManager logManager;

        private readonly EpicIssueMapper epicMapper;

        #endregion

        #region Constructor

        public ImportStoryNoGroupEngine(
            ItemListGetter geminiItemsEngine, 
            StoryIssueMapper geminiToJiraMapper, 
            StorySaveEngine storySaveEngine, 
            AlfrescoUrlsEngine alfrescoEngine, 
            SubtaskSaveEngine subTaskSaveEngine,
            EpicIssueMapper epicMapper,
            LogManager logManager)
        {
            this.geminiItemsEngine = geminiItemsEngine;
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.storySaveEngine = storySaveEngine;
            this.alfrescoEngine = alfrescoEngine;
            this.subTaskSaveEngine = subTaskSaveEngine;
            this.logManager = logManager;
            this.epicMapper = epicMapper;
        }

        #endregion



        

        #region Public methods

        public void Execute(
            GeminiToJiraParameters configurationSetup,
            string projectCode,
            Dictionary<int, Issue> jiraSavedDictionary,
            Dictionary<string, string> storyFolderDictionary,
            string storyType,
            string storySubTaskType,
            List<IssueDto> filteredDevelopments,
            IssueDto geminiIssue)
        {
            try
            {
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);

                //Create Epic
                var jiraEpicInfo = this.epicMapper.Execute(configurationSetup, currentIssue, configurationSetup.Jira.EpicTypeCode, projectCode);

                Issue jiraEpicIssue = this.storySaveEngine.Execute(
                    jiraSavedDictionary,
                    geminiIssue.Id,
                    geminiIssue.Reporter,
                    jiraEpicInfo,
                    configurationSetup);

                var epicStoryFolder = this.alfrescoEngine.Execute(jiraEpicInfo, jiraEpicIssue, "", configurationSetup);

                storyFolderDictionary.Add(jiraEpicIssue.JiraIdentifier, epicStoryFolder);


                //Create Story
                var jiraIssueInfo = geminiToJiraMapper.Execute(configurationSetup, currentIssue, storyType, projectCode);

                jiraIssueInfo.CustomFields.Add(new CustomFieldInfo("Epic Link", jiraEpicIssue.Key.Value));

                Issue jiraIssue = this.storySaveEngine.Execute(
                    jiraSavedDictionary,
                    geminiIssue.Id,
                    geminiIssue.Reporter,
                    jiraIssueInfo,
                    configurationSetup);
                var storyFolder = this.alfrescoEngine.Execute(jiraIssueInfo, jiraIssue, "", configurationSetup);

                storyFolderDictionary.Add(jiraIssue.JiraIdentifier, storyFolder);

                var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != configurationSetup.Gemini.GroupTypeCode && i.Value.Id != currentIssue.Id);

                //Story-SubTask
                foreach (var sub in hierarchy)
                {
                    if (sub.Value.Type == "Task")
                    {
                        //Esclude figli di development (i dev sono censiti come story non in dipendenza
                        if (sub.Value.ParentIssueId.HasValue && sub.Value.ParentIssueId.Value != currentIssue.Id)
                        {
                            var devParentIssue = filteredDevelopments.FirstOrDefault(d => d.Id == sub.Value.ParentIssueId.Value);

                            if (devParentIssue != null)
                                continue;
                        }

                        var currentSubIssue = geminiItemsEngine.Execute(sub.Value.Id);

                        this.subTaskSaveEngine.Execute(
                            configurationSetup,
                            jiraIssue,
                            currentSubIssue,
                            jiraSavedDictionary,
                            storySubTaskType,
                            storyFolder,
                            checkRelease: false);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logManager.Execute(
                    "[Story] - " + geminiIssue.IssueKey + " - " + ex.Message);
            }
        }



        #endregion

        #region Private methods


        #endregion
    }
}
