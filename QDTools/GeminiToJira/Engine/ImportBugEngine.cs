using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.GeminiFilter;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters;
using JiraTools.Engine;
using System.Collections.Generic;
using System.Linq;
using JiraTools.Parameters;
using System;
using Atlassian.Jira;
using System.IO;
using GeminiToJira.Parameters.Import;

namespace GeminiToJira.Engine
{
    public class ImportBugEngine
    {
        private readonly BugIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly JiraTools.Engine.ItemListGetter jiraItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LinkEngine linkEngine;

        public ImportBugEngine(
            BugIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkEngine linkEngine)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraItemsEngine = jiraItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.linkEngine = linkEngine;

        }

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            var projectCode = configurationSetup.JiraProjectCode;

            Countersoft.Gemini.Commons.Entity.IssuesFilter filter = GetBugFilter(configurationSetup);

            var bugIssueList = GetFilteredGeminiIssueList(geminiItemsEngine, filter);

            var geminiBugIssueList = getFiltered(configurationSetup,bugIssueList);

            var bugLogFile = "BugLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            
            foreach (var geminiIssue in geminiBugIssueList.OrderBy(f => f.Id).ToList())
            {
                try
                {
                    var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
            
                    var jiraIssueInfo = geminiToJiraMapper.Execute(configurationSetup, currentIssue, configurationSetup.Jira.BugTypeCode, projectCode, configurationSetup.Gemini.ErmBugPrefix);
            
                    var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo, configurationSetup.Jira, configurationSetup.AttachmentDownloadedPath);
                    SetAndSaveReporter(jiraIssue, geminiIssue,configurationSetup.Jira.DefaultAccount);
                }
                catch
                {
                    File.AppendAllText(configurationSetup.LogDirectory + bugLogFile, geminiIssue.IssueKey + Environment.NewLine);
                }
            
            }
        }

        private IEnumerable<IssueDto> getFiltered(GeminiToJiraParameters configurationSetup, IEnumerable<IssueDto> bugIssueList)
        {
            if (String.IsNullOrWhiteSpace(configurationSetup.Filter.BUG_PRODUCT))
              return bugIssueList;

            return bugIssueList.Where(i => i.CustomFields[11].FormattedData.Equals(configurationSetup.Filter.BUG_PRODUCT));
        }

        #region Private 

        private Countersoft.Gemini.Commons.Entity.IssuesFilter GetBugFilter(GeminiToJiraParameters configurationSetup)
        {
            return new Countersoft.Gemini.Commons.Entity.IssuesFilter()
            {
                IncludeClosed = configurationSetup.Filter.ERMBUG_INCLUDED_CLOSED,
                Projects = configurationSetup.Filter.ERMBUG_PROJECT_ID,
            };
        }


        private IEnumerable<IssueDto> GetFilteredGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            Countersoft.Gemini.Commons.Entity.IssuesFilter filter)
        {
            return geminiItemsEngine.Execute(filter);
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue,string defaultAccount)
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
