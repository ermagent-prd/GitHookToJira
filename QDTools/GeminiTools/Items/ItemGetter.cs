using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Service;

namespace GeminiTools.Items
{
    public class ItemGetter
    {
        private readonly ServiceManagerContainer svc;

        public ItemGetter(ServiceManagerContainer svcFactory)
        {
            this.svc = svcFactory;
        }

        public IssueDto Execute(int issueId)
        {
            return svc.Service.Item.Get(issueId);
        }
    }
}
