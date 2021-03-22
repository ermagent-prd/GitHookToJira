using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    class IssueGeminiCodeGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            var gemini = issue.CustomFields.FirstOrDefault(c => c.Name == "GEMINI");
            return gemini != null ? gemini.Values[0] : "";
        }
    }
}
