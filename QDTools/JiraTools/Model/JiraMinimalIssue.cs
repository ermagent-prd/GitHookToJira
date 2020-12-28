using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromGemini.Jira.Model
{
    public class Project
    {
        public string key { get; set; }
    }

    public class Issuetype
    {
        public string name { get; set; }
    }

    public class Fields
    {
        public Project project { get; set; }
        public Issuetype issuetype { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
    }

    public class JiraMinimalIssue
    {
        public Fields fields { get; set; }
    }
}
