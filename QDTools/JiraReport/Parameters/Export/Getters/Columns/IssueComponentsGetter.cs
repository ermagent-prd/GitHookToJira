using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraReport.Parameters.Export.Getters.Columns
{
    public class IssueComponentsGetter : IExcelFieldGetter
    {
        public string Execute(Issue issue)
        {
            var componets = issue.Components.Select(x => x.Name).ToList();

            return componets != null && componets.Count > 0 ?
                String.Join(", ", componets) : "";
        }
    }
}
