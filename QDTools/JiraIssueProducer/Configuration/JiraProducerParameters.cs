using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraIssueProducer.Configuration
{
    public class JiraProducerParameters
    {
        public CommandLineOptions CommandLineOptions { get; set; }

        public JiraParameters Jira { get; set; }

    }

    public class CommandLineOptions
    {
        public JiraIssueProducerOption ProducerOption { get; set; }

        public string JiraProject { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Attachments { get; set; }

        public string Comment { get; set; }

    }

    public class JiraParameters
    {
        public string JiraUserName { get; set; }

        public string JiraToken { get; set; }

        public string Url { get; set; }

        public string IssueApi { get; set; }

        public int MaxIssuesPerRequest { get; set; }



    }

}
