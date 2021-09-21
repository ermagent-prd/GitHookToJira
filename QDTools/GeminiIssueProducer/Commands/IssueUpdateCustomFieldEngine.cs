using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiIssueProducer.Helpers;
using GeminiIssueProducer.Parameters;

namespace GeminiIssueProducer.Commands
{
    internal class IssueUpdateCustomFieldEngine
    {
        private readonly CustomFieldsHelper customFieldsHelper;

        public IssueUpdateCustomFieldEngine(ServiceManager serviceManager)
        {
            customFieldsHelper = 
                new CustomFieldsHelper(serviceManager);
        }

        public IGeminiCommandOutput Execute(IssueDto newIssue, IssueParams parameters)
        {
            CustomFieldData cfIssueType =
                customFieldsHelper.Execute(
                    newIssue,
                    parameters.FixedParams.ProjectIdValue,
                    parameters.ConfigParams.IssueTypeId,
                    parameters.FixedParams.IssueType,
                    parameters.FixedParams.UserId);

            if (cfIssueType == null)
                return BuildOutput(GeminiConstants.ERR_CANNOT_UPDATE_CF_ISSUETYPE);

            CustomFieldData cfAffectedBuild =
                customFieldsHelper.Execute(
                    newIssue,
                    parameters.FixedParams.ProjectIdValue,
                    parameters.ConfigParams.AffectedBuildId,
                    parameters.FreeParams.AffectedBuild,
                    parameters.FixedParams.UserId);

            if (cfAffectedBuild == null)
                return BuildOutput(GeminiConstants.ERR_CANNOT_UPDATE_CF_BUILD);

            CustomFieldData cfFunctionality =
                customFieldsHelper.Execute(
                    newIssue,
                    parameters.FixedParams.ProjectIdValue,
                    parameters.ConfigParams.FunctionalityId,
                    parameters.FixedParams.Functionality,
                    parameters.FixedParams.UserId);

            if (cfFunctionality == null)
                return BuildOutput(GeminiConstants.ERR_CANNOT_UPDATE_CF_FUNCTIONALITY);

            return
                new SimpleCommandOutput(true, GeminiConstants.OK);
        }

        private SimpleCommandOutput BuildOutput(int errorValue)
        {
            return new SimpleCommandOutput(false, errorValue);
        }
    }
}
