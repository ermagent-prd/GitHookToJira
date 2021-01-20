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
using Atlassian.Jira;
using System;

namespace GeminiToJira
{
    class Program
    {
        private static List<string> userGroups = new List<string>() { "Administrators" };

        private const string StoryTpe = "Story";
        private const string SubTaskType = "Sottotask";
        private const string UatType = "UAT";

        private static readonly string GroupType = "Group";

        static void Main(string[] args)
        {

            var geminiContainer = GeminiContainerFactory.Execute();
            var geminiItemsEngine = geminiContainer.Resolve<ItemListGetter>();

            var jiraContainer = JiraContainer.DefaultInstance.Value;
            var jiraSaveEngine = jiraContainer.Resolve<CreateIssueEngine>();
            var userDictionary = GetUsersDictionary(jiraContainer);

            //var geminiIssueList = filterGeminiIssueList(geminiItemsEngine, FilterType.Development);
            //SaveDevelopmentToJira(geminiItemsEngine, jiraSaveEngine, geminiIssueList, userDictionary);

            var geminiIssueList = filterGeminiIssueList(geminiItemsEngine, FilterType.UAT);
            SaveUatToJira(geminiItemsEngine, jiraSaveEngine, geminiIssueList, userDictionary);

        }

        

        #region Private

        private static Dictionary<string, JiraUser> GetUsersDictionary(IUnityContainer jiraContainer)
        {
            Dictionary<string, JiraUser> result = new Dictionary<string, JiraUser>();

            var usersGetter = jiraContainer.Resolve<UserListGetter>();
            foreach (var group in userGroups)
            {
                var userList = usersGetter.Execute(group);    //TODO da fare per ogni gruppo 
                foreach (var user in userList)
                    result.Add(user.DisplayName, user);
            }

            return result;
        }

        private static IEnumerable<IssueDto> filterGeminiIssueList(ItemListGetter geminiItemsEngine, FilterType filter)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(filter));
            Filter.FilterIssuesList(filter, geminiIssueList);
            return geminiIssueList;
        }

        private static void SaveDevelopmentToJira(
            ItemListGetter geminiItemsEngine, 
            CreateIssueEngine jiraSaveEngine, 
            IEnumerable<IssueDto> geminiIssueList,
            Dictionary<string, JiraUser> userDictionary)
        {
            var geminiToJiraMapper = new DevelopmentIssueMapper();

            foreach (var geminiIssue in geminiIssueList.OrderBy(f => f.Id).ToList())// TODO .Where(i => i.Id == 59840)
            {
                var jiraIssueInfo = geminiToJiraMapper.Execute(geminiIssue, StoryTpe, userDictionary);
                
                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);

                var hierarchy = geminiItemsEngine.Execute(geminiIssue.Id).Hierarchy.Where(i => i.Value.Type != GroupType && i.Value.Id != geminiIssue.Id);

                foreach (var sub in hierarchy)
                {
                    var jiraSubTaskInfo = geminiToJiraMapper.Execute(sub.Value, SubTaskType, userDictionary);
                    jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                    jiraSaveEngine.Execute(jiraSubTaskInfo);
                }
            }
        }

        private static void SaveUatToJira(ItemListGetter geminiItemsEngine, CreateIssueEngine jiraSaveEngine, IEnumerable<IssueDto> geminiIssueList, Dictionary<string, JiraUser> userDictionary)
        {
            var geminiToJiraMapper = new DevelopmentIssueMapper();

            foreach (var geminiIssue in geminiIssueList.Where(i => i.Id == 65514).ToList())// TODO OrderBy(f => f.Id)
            {
                var jiraIssueInfo = geminiToJiraMapper.Execute(geminiIssue, UatType, userDictionary);

                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);

                //var hierarchy = geminiItemsEngine.Execute(geminiIssue.Id).Hierarchy.Where(i => i.Value.Type != GroupType && i.Value.Id != geminiIssue.Id);
                //
                //foreach (var sub in hierarchy)
                //{
                //    var jiraSubTaskInfo = geminiToJiraMapper.Execute(sub.Value, SubTaskType, userDictionary);
                //    jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                //    jiraSaveEngine.Execute(jiraSubTaskInfo);
                //}
            }
        }

        #endregion
    }
}
