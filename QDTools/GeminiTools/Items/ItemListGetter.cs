using System.Collections.Generic;
using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;

namespace GeminiTools.Items
{
    internal class ItemListGetter
    {
        private readonly ServiceManager svc;

        public ItemListGetter(ServiceManager svc)
        {
            this.svc = svc;
        }

        public IEnumerable<IssueDto> Execute(IssuesFilter filter)
        {
            return svc.Item.GetFilteredItems(filter);
        }
    }
}
