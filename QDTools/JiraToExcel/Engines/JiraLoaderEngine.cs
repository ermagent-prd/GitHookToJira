using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Engine;

namespace JiraToExcel.Engines
{
    internal class JiraLoaderEngine
    {

        private readonly JqlGetter getter;

        public JiraLoaderEngine(JqlGetter getter)
        {
            this.getter = getter;
        }

        public IEnumerable<Issue> Execute(string jql)
        {
            return this.getter.Execute(jql);
        }
    }
}
