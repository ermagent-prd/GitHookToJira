using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.GeminiFilter;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters;
using JiraTools.Engine;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;

namespace GeminiToJira.Engine
{
    public class ImportDevelopmentEngine
    {
        private readonly DevelopmentIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        
        public ImportDevelopmentEngine(
            DevelopmentIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;

        }

        public void Execute(string projectCode)
        {
            var geminiDevelopmentIssueList = filterGeminiIssueList(geminiItemsEngine);

            foreach (var geminiIssue in geminiDevelopmentIssueList.OrderBy(f => f.Id).ToList()) // Where(i => i.Id == 59680)
            {
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
                var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.StoryTpe, projectCode);

                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);
                SetAndSaveReporter(jiraIssue, geminiIssue);

                var hierarchy = currentIssue.Hierarchy.Where(i => i.Value.Type != JiraConstants.GroupType && i.Value.Id != currentIssue.Id);

                foreach (var sub in hierarchy)
                {
                    if (sub.Value.Type == "Task")
                    {
                        var jiraSubTaskInfo = geminiToJiraMapper.Execute(sub.Value, JiraConstants.SubTaskType, projectCode);
                        jiraSubTaskInfo.ParentIssueKey = jiraIssue.Key.Value;
                        var subissue = jiraSaveEngine.Execute(jiraSubTaskInfo);
                        SetAndSaveReporter(subissue, sub.Value);
                    }
                }
            }
        }

        #region Private 
        private IEnumerable<IssueDto> filterGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(FilterType.Development));
            Filter.FilterIssuesList(FilterType.Development, geminiIssueList);
            return geminiIssueList;
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter).AccountId;
                jiraIssue.SaveChanges();
            }
        }
        #endregion
    }


}
