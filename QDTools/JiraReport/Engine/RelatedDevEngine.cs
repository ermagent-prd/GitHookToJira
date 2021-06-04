using Atlassian.Jira;
using JiraTools.Engine;
using JiraTools.Parameters;
using System.Collections.Generic;
using System.Linq;

namespace JiraReport.Engine
{
    public class RelatedDevEngine
    {
        private readonly ItemListGetter itemsEngine;
        private readonly Dictionary<string, Issue> relatedDevDictionary;

        public RelatedDevEngine(ItemListGetter itemsEngine)
        {
            this.itemsEngine = itemsEngine;
            relatedDevDictionary = new Dictionary<string, Issue>();
        }

        public Issue Execute(Issue issue)
        {
            if (relatedDevDictionary.TryGetValue(issue.ParentIssueKey, out Issue relatedDev))
                return relatedDev;
            else
            {
                relatedDev = itemsEngine.Execute(issue.ParentIssueKey, QuerableType.ByCode, issue.Project).FirstOrDefault();
                if (relatedDev != null)
                {
                    relatedDevDictionary.Add(relatedDev.Key.Value, relatedDev);

                    return relatedDev;
                }
                else
                    return null;
            }
        }

    }
}
