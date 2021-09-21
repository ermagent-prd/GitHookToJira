using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiIssueProducer.Parameters;
using System;
using System.Collections.Generic;

namespace GeminiIssueProducer.Helpers
{
    internal class FinderHelper
    {
        private readonly ServiceManager serviceManager;

        public FinderHelper(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public IEnumerable<IssueDto> Execute(IssueParams parameters)
        {
            IssuesFilter filter =
                GetGeminiFilter(
                    parameters.FixedParams.ProjectIdValue,
                    parameters.FixedParams.TypeIdValue,
                    parameters.FreeParams.Title,
                    parameters.FixedParams.UserId);
            try
            {
                return
                    serviceManager.Item.GetFilteredItems(filter);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IssuesFilter GetGeminiFilter(
            int projectId,
            int typeId,
            string title,
            int userId)
        {
            var filter = 
                new IssuesFilter();

            filter.IncludeClosed = false;
            //filter.SearchKeywords = "Test keyword";
            filter.Projects = projectId.ToString();
            filter.ReportedByUserId = userId;
            filter.Title = title;
            filter.Types = typeId.ToString();

            return filter;
        }
    }
}
