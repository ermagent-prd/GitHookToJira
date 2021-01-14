using System.Collections.Generic;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiTools.Service;

namespace GeminiTools.Items
{
    public class ItemListGetter
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

        public IssueDto Execute(int issueId)
        {
            return svc.Service.Item.Get(issueId);
        }
    }
}
