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

namespace GeminiToJira
{
    class Program
    {
        private static List<string> userGroups = new List<string>() { "Administrators" };

        static void Main(string[] args)
        {
            var unityContainer = ContainerFactory.Execute();

            var jiraSaveEngine = unityContainer.Resolve<CreateIssueEngine>();            
            var geminiItemsEngine = unityContainer.Resolve<GeminiTools.Items.ItemListGetter>();
            var jiraItemsEngine = unityContainer.Resolve<JiraTools.Engine.ItemListGetter>();
            var userDictionary = GetUsersDictionary(unityContainer);

            var geminiDevelopmentIssueList = filterGeminiIssueList(geminiItemsEngine, FilterType.Development);
            var geminiToJirDevaMapper = unityContainer.Resolve<DevelopmentIssueMapper>();
            SaveDevelopmentToJira(geminiToJirDevaMapper, geminiItemsEngine, jiraSaveEngine, geminiDevelopmentIssueList, userDictionary);

            var geminiUatIssueList = filterGeminiIssueList(geminiItemsEngine, FilterType.UAT);
            var geminiToJiraUatMapper = unityContainer.Resolve<UatIssueMapper>();
            SaveUatToJira(geminiToJiraUatMapper, geminiItemsEngine, jiraItemsEngine, jiraSaveEngine, geminiUatIssueList, userDictionary);

        }

        

        #region Private

        private static Dictionary<string, JiraUser> GetUsersDictionary(IUnityContainer jiraContainer)
        {
            Dictionary<string, JiraUser> result = new Dictionary<string, JiraUser>();

            var usersGetter = jiraContainer.Resolve<UserListGetter>();
            foreach (var group in userGroups)
            {
                var userList = usersGetter.Execute(group);      //TODO da fare per ogni gruppo 
                foreach (var user in userList)
                    result.Add(user.DisplayName, user);         //TODO solo se non già inserito
            }

            return result;
        }

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
            IEnumerable<IssueDto> geminiDevelopmentIssueList,
            Dictionary<string, JiraUser> userDictionary)
        {
            
            foreach (var geminiIssue in geminiDevelopmentIssueList.Where(i => i.Id == 59685))// TODO .OrderBy(f => f.Id).ToList()
            {
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.StoryTpe, userDictionary);
                
                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);

                var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != JiraConstants.GroupType && i.Value.Id != currentIssue.Id);

                foreach (var sub in hierarchy)
                {
                    if (sub.Value.Type == "Task")
                    {
                        var jiraSubTaskInfo = geminiToJiraMapper.Execute(sub.Value, JiraConstants.SubTaskType, userDictionary);
                        jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                        jiraSaveEngine.Execute(jiraSubTaskInfo);
                    }
                }
            }
        }

        private static void SaveUatToJira(
            UatIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine, CreateIssueEngine jiraSaveEngine, IEnumerable<IssueDto> geminiUatIssueList, 
            Dictionary<string, JiraUser> userDictionary)
        {
            var linkEngine = new LinkEngine();

            foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.Id).ToList())
            {
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);           //we need a new call to have the attachments

                var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.UatType, userDictionary);

                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);

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
