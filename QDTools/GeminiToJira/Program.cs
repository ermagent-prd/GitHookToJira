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


namespace GeminiToJira
{
    class Program
    {
        private const string StoryTpe = "Story";
        private const string SubTaskType = "Sottotask";
        private static readonly string GroupType = "Group";

        static void Main(string[] args)
        {
            var geminiToJiraMapper = new GeminiToJiraMapper();

            var geminiContainer = GeminiContainerFactory.Execute();
            var geminiItemsEngine = geminiContainer.Resolve<ItemListGetter>();

            var jiraContainer = JiraContainer.DefaultInstance.Value;
            var jiraSaveEngine = jiraContainer.Resolve<CreateIssueEngine>();

            var geminiIssueList = getGeminiIssueList(geminiItemsEngine, FilterType.Development);

            SaveToJira(geminiToJiraMapper, geminiItemsEngine, jiraSaveEngine, geminiIssueList);
        }


        #region Private
        private static IEnumerable<IssueDto> getGeminiIssueList(ItemListGetter geminiItemsEngine, FilterType filter)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(filter));
            Filter.FilterIssuesList(filter, geminiIssueList);
            return geminiIssueList;
        }

        private static void SaveToJira(
            GeminiToJiraMapper geminiToJiraMapper, 
            ItemListGetter geminiItemsEngine, 
            CreateIssueEngine jiraSaveEngine, 
            IEnumerable<IssueDto> geminiIssueList)
        {
            foreach (var geminiIssue in geminiIssueList.Where(i => i.Id == 59840).ToList())// TODO OrderBy(f => f.Id))
            {
                var jiraIssueInfo = geminiToJiraMapper.Execute(geminiIssue, StoryTpe);
                
                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);

                var hierarchy = geminiItemsEngine.Execute(geminiIssue.Id).Hierarchy.Where(i => i.Value.Type != GroupType && i.Value.Id != geminiIssue.Id);

                foreach (var sub in hierarchy)
                {
                    var jiraSubTaskInfo = geminiToJiraMapper.Execute(sub.Value, SubTaskType);
                    jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                    var jiraSubTask = jiraSaveEngine.Execute(jiraSubTaskInfo);
                }
            }
        }

        #endregion
    }
}
