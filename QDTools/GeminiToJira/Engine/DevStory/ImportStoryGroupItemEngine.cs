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
using JiraTools.Model;

namespace GeminiToJira.Engine
{
    public class ImportStoryGroupItemEngine
    {
        private readonly StoryGroupIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly LogManager logManager;
        private readonly EpicIssueMapper epicMapper;
        private readonly AlfrescoUrlsEngine alfrescoEngine;
        private readonly StorySaveEngine storySaveEngine;
        private readonly SubtaskSaveEngine subtaskSaveEngine;

        private readonly StoryOtherTasksIssueMapper otherTasksStoryMapper;



        public ImportStoryGroupItemEngine(
            StoryGroupIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            LogManager logManager,
            EpicIssueMapper epicMapper,
            AlfrescoUrlsEngine alfrescoEngine,
            StorySaveEngine storySaveEngine,
            SubtaskSaveEngine subtaskSaveEngine,
            StoryOtherTasksIssueMapper otherTasksStoryMapper)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.logManager = logManager;
            this.epicMapper = epicMapper;
            this.alfrescoEngine = alfrescoEngine;
            this.storySaveEngine = storySaveEngine;
            this.subtaskSaveEngine = subtaskSaveEngine;
            this.otherTasksStoryMapper = otherTasksStoryMapper;
        }
        #region Public methods
        public bool Execute(
            GeminiToJiraParameters configurationSetup,
            string projectCode,
            Dictionary<int, Issue> jiraSavedDictionary,
            Dictionary<string, string> storyFolderDictionary,
            string storyType,
            string storySubTaskType,
            List<IssueDto> filteredDevelopments,
            IssueDto geminiDevIssue)
        {
            try
            {
                var currentIssue = this.geminiItemsEngine.Execute(geminiDevIssue.Id);

                //Check group
                var gItem = currentIssue.Hierarchy.Any(i => i.Value.Type == configurationSetup.Gemini.GroupTypeCode);

                if (!gItem)
                    return false;

                //Create Epic
                var jiraEpicInfo = this.epicMapper.Execute(configurationSetup, currentIssue, configurationSetup.Jira.EpicTypeCode, projectCode);

                Issue jiraEpicIssue = this.storySaveEngine.Execute(
                    jiraSavedDictionary,
                    geminiDevIssue.Id,
                    geminiDevIssue.Reporter,
                    jiraEpicInfo, 
                    configurationSetup);

                if (jiraEpicIssue == null)
                    return true;

                var storyFolder = this.alfrescoEngine.Execute(jiraEpicInfo, jiraEpicIssue, "", configurationSetup);

                storyFolderDictionary.Add(jiraEpicIssue.JiraIdentifier, storyFolder);

                var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Id != currentIssue.Id);


                string storyHierarchyKey = string.Format("|{0}|", currentIssue.Id);


                //First Level Task
                var currentIssuesMainTasks = hierarchy.Where(i =>
                    i.Value.Type == configurationSetup.Gemini.TaskTypeCode &&
                    i.Value.HierarchyKey.Equals(storyHierarchyKey));

                if (currentIssuesMainTasks.Any())
                {
                    //Creazione story "Other tasks"
                    var otherTaskStoryInfo = this.otherTasksStoryMapper.Execute(
                        configurationSetup,
                        configurationSetup.Jira.StoryTypeCode, 
                        projectCode,
                        currentIssue,
                        "Other tasks");

                    otherTaskStoryInfo.CustomFields.Add(new CustomFieldInfo("Epic Link", jiraEpicIssue.Key.Value));

                    Issue jiraStoryOtherTaskIssue = this.storySaveEngine.Execute(
                        jiraSavedDictionary, 
                        null,
                        currentIssue.Reporter,
                        otherTaskStoryInfo, 
                        configurationSetup);

                    var subStoryFolder = this.alfrescoEngine.Execute(otherTaskStoryInfo, jiraStoryOtherTaskIssue, "", configurationSetup);

                    storyFolderDictionary.Add(jiraStoryOtherTaskIssue.JiraIdentifier, subStoryFolder);


                    //Creazione subtasks

                    foreach (var task in currentIssuesMainTasks)
                    {
                        var currentSubIssue = geminiItemsEngine.Execute(task.Value.Id);

                        this.subtaskSaveEngine.Execute(
                            configurationSetup,
                            jiraStoryOtherTaskIssue,
                            currentSubIssue,
                            jiraSavedDictionary,
                            storySubTaskType,
                            subStoryFolder,
                            checkRelease: false);

                        string hierarchyKey = string.Format("|{0}|{1}|", currentIssue.Id, currentSubIssue.Id);

                        var subTaskHierarchy = hierarchy.Where(
                            i => i.Value.Type == "Task" &&
                            i.Value.HierarchyKey.Length >= hierarchyKey.Length &&
                            i.Value.HierarchyKey.Substring(0, hierarchyKey.Length) == hierarchyKey);

                        foreach(var othersubTask in subTaskHierarchy)
                        {
                            var subIssue = geminiItemsEngine.Execute(othersubTask.Value.Id);

                            this.subtaskSaveEngine.Execute(
                                configurationSetup,
                                jiraStoryOtherTaskIssue,
                                subIssue,
                                jiraSavedDictionary,
                                storySubTaskType,
                                subStoryFolder,
                                checkRelease: false);

                        }
                    }
                }



                //first level groups
                var currentIssuesGroups = hierarchy.Where(i =>
                    i.Value.Type == configurationSetup.Gemini.GroupTypeCode &&
                    i.Value.HierarchyKey.Equals(storyHierarchyKey));

                //Crea story dai gruppi
                foreach (var group in currentIssuesGroups)
                {

                    var geminiGroupIssue = this.geminiItemsEngine.Execute(group.Value.Id);

                    if (geminiGroupIssue == null)
                        continue;

                    var jiraStoryInfo = this.geminiToJiraMapper.Execute(
                        configurationSetup,
                        geminiGroupIssue, 
                        configurationSetup.Jira.StoryTypeCode, 
                        projectCode,
                        currentIssue);

                    jiraStoryInfo.CustomFields.Add(new CustomFieldInfo("Epic Link", jiraEpicIssue.Key.Value));

                    Issue jiraStoryIssue = this.storySaveEngine.Execute(
                        jiraSavedDictionary, 
                        group.Value.Id,
                        group.Value.Reporter,
                        jiraStoryInfo, 
                        configurationSetup);

                    var subStoryFolder = this.alfrescoEngine.Execute(jiraStoryInfo, jiraStoryIssue, "", configurationSetup);

                    storyFolderDictionary.Add(jiraStoryIssue.JiraIdentifier, subStoryFolder);

                    var geminiGroup = group.Value;

                    if (geminiGroup == null)
                        continue;

                    string hierarchyKey = string.Format("|{0}|{1}|", currentIssue.Id, geminiGroup.Id);

                    var groupHierarchy = hierarchy.Where(
                        i => i.Value.Type != configurationSetup.Gemini.GroupTypeCode && 
                        i.Value.Id != group.Value.Id && i.Value.Type == "Task" &&
                        i.Value.HierarchyKey.Length >= hierarchyKey.Length &&
                        i.Value.HierarchyKey.Substring(0,hierarchyKey.Length) == hierarchyKey);


                    foreach (var sub in groupHierarchy)
                    {

                        //Esclude figli di development (i dev sono censiti come story non in dipendenza
                        if (sub.Value.ParentIssueId.HasValue && sub.Value.ParentIssueId.Value != currentIssue.Id)
                        {
                            var devParentIssue = filteredDevelopments.FirstOrDefault(d => d.Id == sub.Value.ParentIssueId.Value);

                            if (devParentIssue != null)
                                continue;
                        }

                        var currentSubIssue = geminiItemsEngine.Execute(sub.Value.Id);

                        this.subtaskSaveEngine.Execute(
                            configurationSetup,
                            jiraStoryIssue,
                            currentSubIssue,
                            jiraSavedDictionary,
                            storySubTaskType,
                            subStoryFolder,
                            checkRelease: false);
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                this.logManager.Execute(
                    "[Story] - " + geminiDevIssue.IssueKey + " - " + ex.Message);

                return true;
            }
        }

        #endregion

    }


}
