using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Mapper;
using JiraTools.Engine;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using System;
using System.IO;
using GeminiTools.Engine;
using JiraTools.Model;
using GeminiTools.Items;
using AlfrescoTools.Engine;
using DotCMIS.Client;
using GeminiToJira.Parameters.Import;
using JiraTools.Parameters;
using GeminiToJira.Log;
using static GeminiToJira.Engine.JiraAccountIdEngine;

namespace GeminiToJira.Engine
{
    public class ImportTaskEngine
    {
        private readonly TaskIssueMapper geminiToJiraTaskMapper;
        private readonly StoryIssueMapper geminiToJiraSubTaskMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LogManager logManager;


        public ImportTaskEngine(
            TaskIssueMapper geminiToJiraTaskMapper,
            StoryIssueMapper geminiToJiraSubTaskMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LogManager logManager)
        {
            this.geminiToJiraTaskMapper = geminiToJiraTaskMapper;
            this.geminiToJiraSubTaskMapper = geminiToJiraSubTaskMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.logManager = logManager;
        }

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            var projectCode = configurationSetup.JiraProjectCode;
            
            var jiraSavedDictionary = new Dictionary<int, Issue>();

            var taskType = configurationSetup.Jira.TaskTypeCode;
            var subTaskType = configurationSetup.Jira.SubTaskTypeCode;

            var filter = GetTaskFilter(configurationSetup);
            var geminiTaskIssueList = GetFilteredGeminiIssueList(geminiItemsEngine, filter, configurationSetup);

            var taskLogFile = "TaskLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            this.logManager.SetLogFile(configurationSetup.LogDirectory + taskLogFile);

            foreach (var geminiIssue in geminiTaskIssueList.Where(l => l.Type == "Development").OrderBy(f => f.Id).ToList())
            {
                try
                {
                    var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                    var jiraIssueInfo = geminiToJiraTaskMapper.Execute(configurationSetup, currentIssue, taskType, projectCode);

                    //Create task
                    Issue jiraIssue = SaveAndSetTask(jiraSavedDictionary, geminiIssue, jiraIssueInfo, configurationSetup);

                    var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != configurationSetup.Gemini.GroupTypeCode && i.Value.Id != currentIssue.Id);

                    //SubTask
                    foreach (var sub in hierarchy)
                    {
                        if (sub.Value.Type == "Task")
                        {
                            var currentSubIssue = geminiItemsEngine.Execute(sub.Value.Id);

                            SaveAndSetSubTask(configurationSetup, jiraIssue, currentSubIssue, jiraSavedDictionary, subTaskType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logManager.Execute("[Task] - " + geminiIssue.IssueKey + " - " + ex.Message);
                }
            }

            #region Orphans
            //manage orphans if exist (an orphan is a gemini task that was not listed in the hierarhy of a Story, that was previous inserted
            foreach (var geminiIssue in geminiTaskIssueList.Where(l => l.Type == "Task").OrderBy(f => f.Id).ToList())
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
                            var jiraIssueInfo = geminiToJiraTaskMapper.Execute(configurationSetup, currentSubIssue, taskType, projectCode);
                            Issue jiraIssue = SaveAndSetTask(jiraSavedDictionary, geminiIssue, jiraIssueInfo, configurationSetup);

                            //and become a subtask of myself
                            SaveAndSetSubTask(configurationSetup, jiraIssue, currentSubIssue, jiraSavedDictionary, subTaskType);

                        }
                        else
                        {
                            //my father was inserted and i was not in his hierarchy list
                            var fatherKey = currentSubIssue.HierarchyKey.Split('|').ElementAt(1);

                            if (jiraSavedDictionary.TryGetValue(Convert.ToInt32(fatherKey), out Issue jiraFatherIssue))
                                SaveAndSetSubTask(configurationSetup, jiraFatherIssue, currentSubIssue, jiraSavedDictionary, subTaskType);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logManager.Execute("[SubTask] - " + currentSubIssue.IssueKey + " - " + ex.Message);
                    }
                }
            }

            #endregion
        }

        private Issue SaveAndSetTask(Dictionary<int, Issue> jiraSavedDictionary, IssueDto geminiIssue, CreateIssueInfo jiraIssueInfo, GeminiToJiraParameters configurationSetup)
        {
            //save story
            var jiraIssue = jiraSaveEngine.Execute(
                jiraIssueInfo, 
                configurationSetup.Jira, 
                configurationSetup.AttachmentDownloadedPath);
            //and set as saved
            if(!jiraSavedDictionary.TryGetValue(geminiIssue.Id, out Issue existing))
                jiraSavedDictionary.Add(geminiIssue.Id, jiraIssue);

            //save reporter
            SetAndSaveReporter(jiraIssue, geminiIssue, configurationSetup.Jira.DefaultAccount);
            return jiraIssue;
        }

        private void SaveAndSetSubTask(
            GeminiToJiraParameters configurationSetup,
            Issue jiraIssue,
            IssueDto currentSubIssue,
            Dictionary<int, Issue> jiraSavedDictionary,
            string subTaskType)
        {
            if (CheckIfValidItem(currentSubIssue, configurationSetup))
            {
                var jiraSubTaskInfo = geminiToJiraSubTaskMapper.Execute(configurationSetup, currentSubIssue, subTaskType, configurationSetup.JiraProjectCode);

                jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;

                //create subtask
                var subIssue = jiraSaveEngine.Execute(
                    jiraSubTaskInfo, 
                    configurationSetup.Jira, 
                    configurationSetup.AttachmentDownloadedPath);
                
                //and set as saved
                if (!jiraSavedDictionary.TryGetValue(currentSubIssue.Id, out Issue existing))
                    jiraSavedDictionary.Add(currentSubIssue.Id, subIssue);

                SetAndSaveReporter(subIssue, currentSubIssue, configurationSetup.Jira.DefaultAccount);
            }
        }



        #region Private 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geminiIssue"></param>
        /// <returns></returns>
        private bool CheckIfValidItem(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Filter.TASK_RELEASE_KEY);
            var devLine = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Filter.TASK_LINE_KEY);

            if (release != null && devLine != null &&
                configurationSetup.Filter.TASK_RELEASES.Contains(release.FormattedData) &&
                configurationSetup.Filter.TASK_LINES.Contains(devLine.FormattedData))
                return true;
            else
                return false;

        }

        private IEnumerable<IssueDto> GetFilteredGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            Countersoft.Gemini.Commons.Entity.IssuesFilter filter,
            GeminiToJiraParameters configurationSetup)
        {
            var geminiIssueList = geminiItemsEngine.Execute(filter);

            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in geminiIssueList.OrderBy(f => f.Id))
            {
                if(CheckIfValidItem(l, configurationSetup))
                    filteredList.Add(l);
            }

            return filteredList;
        }

        private Countersoft.Gemini.Commons.Entity.IssuesFilter GetTaskFilter(GeminiToJiraParameters configurationSetup)
        {
            return new Countersoft.Gemini.Commons.Entity.IssuesFilter
            {
                IncludeClosed = configurationSetup.Filter.TASK_INCLUDED_CLOSED,
                Projects = configurationSetup.Filter.TASK_PROJECT_ID,
                Types = configurationSetup.Filter.TASK_TYPES,
            };
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue, string defaultAccount)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter, defaultAccount).AccountId;
                jiraIssue.SaveChanges();
            }
        }

        
        #endregion
    }


}
