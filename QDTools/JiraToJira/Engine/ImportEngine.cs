using Atlassian.Jira;
using JiraToJira.Container.Jira;
using JiraTools.Engine;
using JiraTools.Parameters;
using JiraTools.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraToJira.Engine
{
    public class ImportEngine
    {
        private readonly CloneIssueEngine cloneIssueEngine;
        private readonly JqlGetter jqlEngine;

        //Original search engine
        private readonly ItemListGetter searchJiraOriginalItemsEngine;

        //Destinantion search engine
        private readonly ItemListGetter searchJiraDestItemsEngine;

        public ImportEngine(JqlGetter jqlEngine, CloneIssueEngine cloneIssueEngine, ItemListGetter jiraItemsEngine, LinkEngine linkEngine)
        {
            this.cloneIssueEngine = cloneIssueEngine;
            this.jqlEngine = jqlEngine;
            this.searchJiraOriginalItemsEngine = jiraItemsEngine;

            this.searchJiraDestItemsEngine = GetJiraDestItemsEngine();
        }

        internal void Execute(string fromProject, string destProject, string jqlSearch)
        {
            var issueList = jqlEngine.Execute(jqlSearch);

            foreach (var issue in issueList)
            {
                Issue relatedDev = GetRelatedDevelopment(issue.ParentIssueKey, destProject, fromProject);

                var clonedIssue = cloneIssueEngine.Execute(issue, destProject, relatedDev);
                
                clonedIssue.SaveChanges();


                if (issue.ParentIssueKey == null || issue.ParentIssueKey == "")
                {
                    var epicLink = issue.CustomFields.FirstOrDefault(x => x.Name == "Epic Link");
                    if (epicLink != null && epicLink.Values[0] != "")
                    {
                        relatedDev = GetRelatedDevelopment(epicLink.Values[0], destProject, fromProject);
                        clonedIssue.CustomFields.Add("Epic Link", relatedDev.Key.Value);
                        clonedIssue.SaveChanges();
                    }
                }
            }
        }

        private ItemListGetter GetJiraDestItemsEngine()
        {
            var parContainer = new JiraToJiraParamContainer();
            var serviceManagerContainer = new ServiceManagerContainer(parContainer);
            return new ItemListGetter(serviceManagerContainer, parContainer);
        }

        private Issue GetRelatedDevelopment(string parentIssueKey, string destProject, string fromProject)
        {
            if (parentIssueKey == "" || parentIssueKey == null)
                return null;

            Issue jiraDev = null;

            var jiraOriginalDevList = searchJiraOriginalItemsEngine.Execute(parentIssueKey, QuerableType.ByCode, fromProject).ToList();

            if(jiraOriginalDevList != null && jiraOriginalDevList.Count > 0)
            {
                var jiraNewDevList = searchJiraDestItemsEngine.Execute(RemoveSpecialChar(jiraOriginalDevList[0].Summary), QuerableType.BySummary, destProject).ToList();

                if (jiraNewDevList != null && jiraNewDevList.Count > 0)
                    jiraDev = jiraNewDevList[0];
            }

            return jiraDev;
        }

        private string RemoveSpecialChar(string summary)
        {
            var search = summary.Replace("\"", "");
            search = search.Replace("[", "");
            search = search.Replace("]", "");
            search = search.Replace("+", "");
            search = search.Replace("-", "");

            return search;
        }
    }
}
