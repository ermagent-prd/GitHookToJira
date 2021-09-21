using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using System.Linq;

namespace GeminiIssueProducer.Helpers
{
    internal class CustomFieldsHelper
    {
        private readonly ServiceManager serviceManager;

        public CustomFieldsHelper(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public CustomFieldData Execute(
            IssueDto newIssue,
            int projectId,
            int customFieldId,
            string data,
            int currentUserId)
        {
            CustomFieldDataDto cff =
                newIssue.CustomFields.FirstOrDefault(
                    cf =>
                    cf.Entity.CustomFieldId == customFieldId);

            if (cff != null)
            {
                var customFieldData = new CustomFieldData();

                customFieldData.Id = cff.Entity.Id;

                customFieldData.ProjectId = projectId;
                customFieldData.CustomFieldId = customFieldId;
                customFieldData.Data = data;
                customFieldData.IssueId = newIssue.Id;
                customFieldData.UserId = currentUserId;

                try
                {
                    return
                        serviceManager.Item.CustomFieldDataUpdate(customFieldData);
                }
                catch (System.Exception)
                {
                    return null;
                }
            }

            // do nothing: the field is supposed to already exist
            return null;
        }
    }
}
