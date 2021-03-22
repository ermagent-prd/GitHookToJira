using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraReport.Engine;
using JiraTools.Engine;
using JiraTools.Parameters;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueRelatedDevKeyCodeCodeGetter : IExcelFieldGetter
    {
        private readonly RelatedDevEngine relatedDevEngine;

        public IssueRelatedDevKeyCodeCodeGetter(RelatedDevEngine relatedDevEngine)
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
