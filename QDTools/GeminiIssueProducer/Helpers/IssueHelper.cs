using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiIssueProducer.Parameters;
using System;
using System.Collections.Generic;

namespace GeminiIssueProducer.Helpers
{
    internal class IssueHelper
    {
        private readonly ServiceManager serviceManager;

        public IssueHelper(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        internal IssueDto Execute(IssueParams parameters, List<int> resourceUserIds)
        {
            try
            {
                var issue = new Issue();

                issue.ProjectId = parameters.FixedParams.ProjectIdValue;
                issue.TypeId = parameters.FixedParams.TypeIdValue;
                issue.Title = parameters.FreeParams.Title;
                issue.Description = parameters.FreeParams.Description;
                issue.SeverityId = parameters.FixedParams.SeverityIdValue;

                AddResources(issue, resourceUserIds);

                return serviceManager.Item.Create(issue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void AddResources(Issue issue, List<int> resourceUserIds)
        {
            foreach (int resourceUserId in resourceUserIds)
            {
                issue.AddResource(resourceUserId);
            }
        }
    }
}
