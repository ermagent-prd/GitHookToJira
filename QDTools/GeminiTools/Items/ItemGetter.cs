using Countersoft.Gemini.Commons.Dto;

namespace GeminiTools
{
    public class ItemGetter
    {
        private readonly ServiceManagerFactory svcFactory;

        public ItemGetter(ServiceManagerFactory svcFactory)
        {
            this.svcFactory = svcFactory;
        }

        public IssueDto Execute(string geminiUrl, int issueId)
        {
            var svc = this.svcFactory.Execute(geminiUrl);

            return svc.Item.Get(issueId);
        }
    }
}
