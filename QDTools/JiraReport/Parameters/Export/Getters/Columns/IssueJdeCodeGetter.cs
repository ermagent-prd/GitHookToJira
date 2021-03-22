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
    public class IssueJdeCodeGetter : IExcelFieldGetter
    {
        private readonly RelatedDevEngine relatedDevEngine;

        public IssueJdeCodeGetter(RelatedDevEngine relatedDevEngine)
        {
            this.relatedDevEngine = relatedDevEngine;

        }
        public string Execute(Issue issue)
        {
            var relatedDev = relatedDevEngine.Execute(issue);

            if (relatedDev != null)
            {
                var jdeCode = relatedDev.CustomFields.FirstOrDefault(c => c.Name == "JDE Module");
                return jdeCode != null ? jdeCode.Values[0] : "";
            }

            return "";
        }
    }
}
