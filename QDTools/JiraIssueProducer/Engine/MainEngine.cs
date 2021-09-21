using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraIssueProducer.Configuration;

namespace JiraIssueProducer.Engine
{
    internal class MainEngine
    {
        private readonly ConfigurationContainer configurationManager;

        public MainEngine(ConfigurationContainer configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public int Execute()
        {
            throw new NotImplementedException();
        }
    }
}
