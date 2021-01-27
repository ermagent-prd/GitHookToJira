using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.GeminiFilter;
using GeminiToJira.Mapper;
using GeminiTools.Items;
using GeminiTools.Container;
using JiraTools.Engine;
using JiraTools.Container;
using System.Collections.Generic;
using System.Linq;
using Unity;
using GeminiToJira.Container;
using Atlassian.Jira;
using JiraTools.Parameters;
using GeminiToJira.Parameters;
using System;
using GeminiToJira.Engine;

namespace GeminiToJira
{
    class Program
    {   
        private static JiraAccountIdEngine accountEngine;

        static void Main(string[] args)
        {
            var unityContainer = ContainerFactory.Execute();

            accountEngine = unityContainer.Resolve<JiraAccountIdEngine>();
            var jiraSaveEngine = unityContainer.Resolve<CreateIssueEngine>();            
            var geminiItemsEngine = unityContainer.Resolve<GeminiTools.Items.ItemListGetter>();
            var jiraItemsEngine = unityContainer.Resolve<JiraTools.Engine.ItemListGetter>();

            //var geminiDevelopmentIssueList = filterGeminiIssueList(geminiItemsEngine, FilterType.Development);
            //var geminiToJirDevaMapper = unityContainer.Resolve<DevelopmentIssueMapper>();
            //SaveDevelopmentToJira(geminiToJirDevaMapper, geminiItemsEngine, jiraSaveEngine, geminiDevelopmentIssueList);

            var geminiUatIssueList = filterGeminiIssueList(geminiItemsEngine, FilterType.UAT);
            var geminiToJiraUatMapper = unityContainer.Resolve<UatIssueMapper>();
            SaveUatToJira(geminiToJiraUatMapper, geminiItemsEngine, jiraItemsEngine, jiraSaveEngine, geminiUatIssueList);

        }

        

        #region Private

        private static IEnumerable<IssueDto> filterGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine, 
            FilterType filter)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(filter));
            Filter.FilterIssuesList(filter, geminiIssueList);
            return geminiIssueList;
        }

        private static void SaveDevelopmentToJira(
            DevelopmentIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine, 
            CreateIssueEngine jiraSaveEngine, 
            IEnumerable<IssueDto> geminiDevelopmentIssueList)
        {            
            foreach (var geminiIssue in geminiDevelopmentIssueList.Where(i => i.Id == 59685))// TODO .OrderBy(f => f.Id).ToList()
            {
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.StoryTpe);
                
                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);
                SetAndSaveReporter(jiraIssue, geminiIssue);

                var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != JiraConstants.GroupType && i.Value.Id != currentIssue.Id);

                foreach (var sub in hierarchy)
                {
                    if (sub.Value.Type == "Task")
                    {
                        var jiraSubTaskInfo = geminiToJiraMapper.Execute(sub.Value, JiraConstants.SubTaskType);
                        jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                        var subissue = jiraSaveEngine.Execute(jiraSubTaskInfo);
                        SetAndSaveReporter(subissue, sub.Value);
                    }
                }
            }
        }

        private static void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter);
                jiraIssue.SaveChanges();
            }
        }

        private static void SaveUatToJira(
            UatIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine, 
            CreateIssueEngine jiraSaveEngine, 
            IEnumerable<IssueDto> geminiUatIssueList)
        {
            var linkEngine = new LinkEngine();

            foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.Id).ToList())
            {
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);           //we need a new call to have the attachments

                var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.UatType);

                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);
                SetAndSaveReporter(jiraIssue, geminiIssue);

                if (jiraIssueInfo.RelatedDevelopment != null && jiraIssueInfo.RelatedDevelopment != "")
                {
                    Issue relatedDev = GetRelatedDevelopment(jiraItemsEngine, jiraIssueInfo);

                    if (relatedDev != null)
                        linkEngine.Execute(jiraIssue, relatedDev.Key.ToString(), "Relates");
                }
            }
        }

        private static Issue GetRelatedDevelopment(JiraTools.Engine.ItemListGetter jiraItemsEngine, JiraTools.Model.CreateIssueInfo jiraIssueInfo)
        {
            Issue jiraDev = null;
            var jiraDevList = jiraItemsEngine.Execute(jiraIssueInfo.RelatedDevelopment, QuerableType.BySummary);

            foreach(var curr in jiraDevList)
            {
                var geminiId = curr.CustomFields.FirstOrDefault(j => j.Name == "GEMINI");
                if (geminiId != null && geminiId.Values[0] == jiraIssueInfo.RelatedDevelopmentId)
                    jiraDev = curr;
            }

            return jiraDev;
        }

        #endregion
    }
}
