using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    public class GeminiConfiguration
    {
        public string Url { get; set; }
        public string ErmBugProjectCode { get; set; }
        public string ErmPrefix { get; set; }
        public string UatPrefix { get; set; }
        public string ErmBugPrefix { get; set; }
        public string ProjectUrl { get; set; }
        public string GroupTypeCode { get; set; }
    }
}
