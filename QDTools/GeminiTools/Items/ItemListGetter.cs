using System.Collections.Generic;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;

namespace GeminiTools.Items
{
    internal class ItemListGetter
    {
        private readonly ServiceManagerFactory svcFactory;

        public ItemListGetter(ServiceManagerFactory svcFactory)
        {
            this.svcFactory = svcFactory;
        }

        public IEnumerable<IssueDto> Execute(string geminiUrl, IssuesFilter filter)
        {
            var svc = this.svcFactory.Execute(geminiUrl);

            return svc.Item.GetFilteredItems(filter);
        }
    }
}
