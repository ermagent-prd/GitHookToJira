using Countersoft.Gemini.Api;
using GeminiIssueProducer.CommandOptions;
using System;
using System.Collections.Generic;

namespace GeminiIssueProducer
{
    internal class GeminiCommandFactory
    {
        private Dictionary<GeminiIssueProducerOptions, Lazy<IGeminiCommand>> commands;

        public GeminiCommandFactory(ServiceManager serviceManager)
        {
            Init(serviceManager);
        }

        public IGeminiCommand Get(GeminiIssueProducerOptions option)
        {
            if (commands.ContainsKey(option))
                return commands[option].Value;

            return null;
        }

        private void Init(ServiceManager serviceManager)
        {
            commands = new Dictionary<GeminiIssueProducerOptions, Lazy<IGeminiCommand>>();

            commands.Add(
                GeminiIssueProducerOptions.AddOrUpdate,
                new Lazy<IGeminiCommand>(() => new IssueAddOrUpdate(serviceManager)));

            commands.Add(
                GeminiIssueProducerOptions.UpdateNotClosed,
                new Lazy<IGeminiCommand>(() => new IssueUpdateNotClosed(serviceManager)));

        }



    }
}
