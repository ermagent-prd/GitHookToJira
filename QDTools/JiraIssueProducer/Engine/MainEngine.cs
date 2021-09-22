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

        private readonly IUpdateNotClosedEngine updateEngine;

        private readonly IAddOrUpdateEngine addOrUpdateEngine;

        public MainEngine(
            ConfigurationContainer configurationManager,
            IUpdateNotClosedEngine updateEngine,
            IAddOrUpdateEngine addOrUpdateEngine)
        {
            this.configurationManager = configurationManager;

            this.addOrUpdateEngine = addOrUpdateEngine;

            this.updateEngine = updateEngine;
        }

        public int Execute()
        {
            if (this.configurationManager.Configuration.CommandLineOptions.ProducerOption == JiraIssueProducerOption.AddOrUpdate)
                return this.addOrUpdateEngine.Execute();

            if (this.configurationManager.Configuration.CommandLineOptions.ProducerOption == JiraIssueProducerOption.UpdateNotClosed)
                return this.updateEngine.Execute();

            return CommandOptions.ERR_WRONG_OPTION;
        }
    }
}
