using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReport.Parameters.Export.Getters
{
    internal interface IExcelFieldGetter
    {
        string Execute(Issue issue);
    }
}
