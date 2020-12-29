using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;

namespace GeminiTools
{
    public class ItemGetter
    {
        private readonly ServiceManager svc;

        public ItemGetter(ServiceManager svcFactory)
        {
            this.svc = svcFactory;
        }

        public IssueDto Execute(int issueId)
        {
            return svc.Item.Get(issueId);
        }
    }
}
