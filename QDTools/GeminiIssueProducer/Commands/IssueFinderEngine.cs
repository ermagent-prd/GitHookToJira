using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using GeminiIssueProducer.Helpers;
using GeminiIssueProducer.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiIssueProducer.Commands
{
    internal class IssueFinderEngine
    {
        private readonly FinderHelper finderHelper;

        public IssueFinderEngine(ServiceManager serviceManager)
        {
            finderHelper =
                new FinderHelper(serviceManager);
        }

        public FindCommandOutput Execute(IssueParams parameters)
        {
            bool result = true;
            int errCode = GeminiConstants.OK;
            int[] resultIds = Array.Empty<int>();

            if (String.IsNullOrWhiteSpace(parameters.FreeParams.Title))
                return new FindCommandOutput(false, GeminiConstants.ERR_EMPTY_TITLE, resultIds);

            IEnumerable<IssueDto> foundItems =
                finderHelper.Execute(parameters);

            if (foundItems == null)
            {
                result = false;
                errCode = GeminiConstants.ERR_FIND_FAIL;
            }
            else
            {
                resultIds =
                    CustomFilter(foundItems, parameters);

                if (!resultIds.Any())
                    errCode = GeminiConstants.NOT_FOUND;
            }
            
            return
                new FindCommandOutput(
                   result,
                   errCode,
                   resultIds);
        }

        private int[] CustomFilter(IEnumerable<IssueDto> items, IssueParams parameters)
        {
            return
                items
                .Where(
                    i =>
                    SameIssueType(i, parameters.ConfigParams.IssueTypeId, parameters.FixedParams.IssueType) &&
                    i.Title.Equals(parameters.FreeParams.Title))
                .Select(i => i.Id)
                .ToArray();
        }

        private bool SameIssueType(IssueDto toTest, int issueTypeId, string value)
        {
            CustomFieldDataDto cff =
                toTest.CustomFields.FirstOrDefault(
                    cf =>
                    cf.Entity.CustomFieldId == issueTypeId);

            if (cff != null)
                return cff.Entity.Data == value;

            return false;
        }
    }
}
