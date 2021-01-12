using GeminiToJira.Container;
using GeminiToJira.GeminiFilter;
using GeminiTools.Items;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GeminiToJira
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var geminiContainer = GeminiContainer.DefaultInstance.Value;

            var devFilter = new DevelopmentFilter();

            //var engine = geminiContainer.Resolve<ItemListGetter>();
            //
            //var list = engine.Execute(devFilter.GetDevelopmentFilter());

        }
    }
}
