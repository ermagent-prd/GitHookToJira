using Atlassian.Jira;
using JiraToJira.Container.Jira;
using JiraTools.Engine;
using JiraTools.Parameters;
using JiraTools.Service;
using System;
using System.IO;
using System.Linq;

namespace JiraToJira.Engine
{
    public class ImportEngine
    {
        private readonly CloneIssueEngine cloneIssueEngine;
        private readonly IssueLinkSearchEngine issueLinkedSearchEngine;
        private readonly LinkEngine linkEngine;

        //Original search engine
        private readonly ItemListGetter searchJiraOriginalItemsEngine;
        private readonly JqlGetter jqlOriginalEngine;

        //Destinantion search engine
        private readonly JqlGetter jqlDestItemsEngine;

        public ImportEngine(JqlGetter jqlEngine, 
            CloneIssueEngine cloneIssueEngine, 
            ItemListGetter jiraItemsEngine, 
            LinkEngine linkEngine, IssueLinkSearchEngine issueLinkearchEngine)
        {
            this.cloneIssueEngine = cloneIssueEngine;

            this.issueLinkedSearchEngine = issueLinkearchEngine;
            this.linkEngine = linkEngine;

            this.jqlOriginalEngine = jqlEngine;
            this.searchJiraOriginalItemsEngine = jiraItemsEngine;

            this.jqlDestItemsEngine = GetJiraDestJqlGetterEngine();
        }

        internal void Execute(string fromProjectCode, string destProjectCode, string fromProjectName, string destProjectName, string type)
        {
            var jqlSearch = "project = \"" + fromProjectName + "\" AND type = " + type + " ORDER BY key ASC";

            string outputFilePath = "C:\\GeminiPorting\\Log\\JiraToJira\\";

            var originalIssueList = jqlOriginalEngine.Execute(jqlSearch);

            var datetime = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");
            string mappingFileName = "Migration" + "_" + destProjectCode + "_" + type + "_" + datetime + ".txt";
            string exceptionFileName = "MigrationException" + "_" + destProjectCode + "_" + type + "_" + datetime + ".txt";

            var oFile = Path.Combine(outputFilePath, mappingFileName);
            var eFile = Path.Combine(outputFilePath, exceptionFileName);

            File.AppendAllText(oFile, jqlSearch + Environment.NewLine);
            File.AppendAllText(eFile, jqlSearch + Environment.NewLine);

            foreach (var originalIssue in originalIssueList)
            {
                Issue relatedDev = GetRelatedDevelopment(originalIssue.ParentIssueKey, destProjectName, fromProjectCode);
                Issue relatedEpicDev = GetEpicRelatedDev(fromProjectCode, destProjectName, originalIssue);

                var clonedIssue = cloneIssueEngine.Execute(originalIssue, destProjectCode, relatedDev);

                if (clonedIssue == null)
                {
                    File.AppendAllText(oFile, "During " + type + " import failed to save " + originalIssue.Key.Value + "because parent value is null" + Environment.NewLine);
                    continue;
                }

                if (relatedEpicDev != null)
                {
                    clonedIssue.CustomFields.Add("Epic Link", relatedEpicDev.Key.Value);
                    clonedIssue.SaveChanges();
                }

                ManageRelatedLinks(destProjectName, originalIssue, clonedIssue);

                File.AppendAllText(oFile, originalIssue.Key.Value + ";" + clonedIssue.Key.Value + Environment.NewLine);


            }
        }


        private void ManageRelatedLinks(string destProjectName, Issue originalIssue, Issue clonedIssue)
        {
            //find linked items to the originalIssue 
            var linkedItems = issueLinkedSearchEngine.Execute(originalIssue);

            foreach (var linkedItem in linkedItems)
            {
                //find related issue to the new environment
                var jsql = "project = \"" + destProjectName + "\" and summary ~ \"" + RemoveSpecialChar(linkedItem.InwardIssue.Summary) + "\" and type = " + linkedItem.InwardIssue.Type.Name + " ORDER BY key ASC";
                var issuesToLink = jqlDestItemsEngine.Execute(jsql).ToList();

                if (issuesToLink != null && issuesToLink.Count > 0)
                {
                    foreach (var issueToLink in issuesToLink)
                    {
                        if (clonedIssue.Key.Value != issueToLink.Key.ToString())
                            linkEngine.Execute(clonedIssue, issueToLink.Key.ToString(), "Relates");
                    }
                }
            }
        }

        private Issue GetEpicRelatedDev(string fromProject, string destProjectName, Issue issue)
        {
            Issue relatedEpicDev = null;
            if (issue.ParentIssueKey == null || issue.ParentIssueKey == "")
            {
                var epicLink = issue.CustomFields.FirstOrDefault(x => x.Name == "Epic Link");
                if (epicLink != null && epicLink.Values[0] != "")
                {
                    relatedEpicDev = GetRelatedDevelopment(epicLink.Values[0], destProjectName, fromProject);
                }

            }

            return relatedEpicDev;
        }

        private JqlGetter GetJiraDestJqlGetterEngine()
        {
            var parContainer = new JiraToJiraParamContainer();
            var serviceManagerContainer = new ServiceManagerContainer(parContainer);
            return new JqlGetter(serviceManagerContainer, parContainer);
        }

        private Issue GetRelatedDevelopment(string parentIssueKey, string destProjectName, string fromProject)
        {
            if (parentIssueKey == "" || parentIssueKey == null)
                return null;

            Issue jiraDev = null;

            var jiraOriginalDevList = searchJiraOriginalItemsEngine.Execute(parentIssueKey, QuerableType.ByCode, fromProject).ToList();

            if(jiraOriginalDevList != null && jiraOriginalDevList.Count > 0)
            {
                var key = jiraOriginalDevList[0].Key.Value;
                var devType = jiraOriginalDevList[0].Type.Name;
                var summary = jiraOriginalDevList[0].Summary;

                var jsql = "project = \"" + destProjectName + "\" and summary ~ \"" + RemoveSpecialChar(summary) + "\" and type = " + devType + " ORDER BY key ASC";
                var issueList = jqlDestItemsEngine.Execute(jsql);

                if (issueList != null)
                {
                    foreach (var i in issueList)
                    {
                        var originalKey = i.CustomFields.First(x => x.Name == "OriginalKey").Values[0];

                        if (originalKey == key)
                            jiraDev = i;
                    }
                }
            }

            return jiraDev;
        }

        private string RemoveSpecialChar(string summary)
        {
            var search = summary.Replace("\"", "");
            search = search.Replace(" [", " ");
            search = search.Replace("[", "");
            search = search.Replace("] ", " ");
            search = search.Replace("]", "");
            search = search.Replace(" : ", " ");
            search = search.Replace(": ", " ");
            search = search.Replace(":", " ");            
            search = search.Replace(" , ", " ");
            search = search.Replace(", ", " ");
            search = search.Replace(",", " ");
            search = search.Replace(" + ", " ");
            search = search.Replace("+", " ");
            search = search.Replace(" - ", " ");

            return search;
        }
    }
}
