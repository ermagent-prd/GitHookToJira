using Atlassian.Jira;
using JiraReport.Engine;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueRelatedDevKeyCodeGetter : IExcelFieldGetter
    {
        private readonly RelatedDevEngine relatedDevEngine;

        public IssueRelatedDevKeyCodeGetter(RelatedDevEngine relatedDevEngine)
        {
            this.relatedDevEngine = relatedDevEngine;

        }

        public string Execute(Issue issue)
        {
            var relatedDev = relatedDevEngine.Execute(issue);

            if (relatedDev != null)
                return relatedDev.Key.Value;

            return "";
        }
    }
}
