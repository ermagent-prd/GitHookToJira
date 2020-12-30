using System.Collections.Generic;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiTools.Service;

namespace GeminiTools.Items
{
    internal class ItemListGetter
    {
        private readonly ServiceManagerContainer svc;

        public ItemListGetter(ServiceManagerContainer svc)
        {
            this.svc = svc;
        }

        public IEnumerable<IssueDto> Execute(IssuesFilter filter)
        {
            return svc.Service.Item.GetFilteredItems(filter);
        }
    }
}
