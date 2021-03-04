using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    public class FilterConfiguration
    {
        //from DevelopmentConstants.cs
        //from ErmBugConstants.cs
        //from UatConstants.cs
        public string DEVELOPMENT_PROJECT_ID { get; set; }
        public string DEVELOPMENT_TYPES { get; set; }
        public List<string> DEVELOPMENT_RELEASES { get; set; }
        public List<string> DEVELOPMENT_LINES { get; set; }
        public string DEVELOPMENT_RELEASE_KEY { get; set; }
        public string DEVELOPMENT_LINE_KEY { get; set; }
        public bool DEVELOPMENT_INCLUDED_CLOSED { get; set; }
        public string UAT_PROJECT_ID { get; set; }
        public string UAT_CREATED_FROM { get; set; }
        public bool UAT_GROUP_DEPENDENCIES { get; set; }
        public bool UAT_INCLUDED_CLOSED { get; set; }
        public List<string> UAT_FUNCTIONALITY { get; set; }
        public string ERMBUG_PROJECT_ID { get; set; }
        public bool ERMBUG_INCLUDED_CLOSED { get; set; }

    }
}
