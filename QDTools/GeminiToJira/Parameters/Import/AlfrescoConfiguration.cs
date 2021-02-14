using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    public class AlfrescoConfiguration
    {
        public string ServiceUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RootFolder { get; set; }
    }
}
