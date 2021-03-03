using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    public class JiraConfiguration
    {
        public string User { get; set; }

        public string Token { get; set; }

        public string Url { get; set; }

        public string IssueApi { get; set; }

        public int MaxIssuesPerRequest { get; set; }


        /// <summary>
        /// 10014:UAT,10004:Bug,10000:Epic,10001:Story,10002:Task,10003:Sub-task]
        /// </summary>

        //to set , from Json configuration, for import (i.e. ERM vs ERM SHL Style Sample Project (SSSP)...etc)
        public string EpicCode { get; set; }
        public string StoryTypeCode { get; set; }
        public string TaskCode { get; set; }
        public string SubTaskTypeCode { get; set; }
        public string UatTypeCode { get; set; }
        public string BugTypeCode { get; set; }
        
    }
}
