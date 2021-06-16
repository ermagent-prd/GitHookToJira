using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTools.Parameters
{
    public class MappingConfiguration
    {
        public string AFFECTEDBUILD_LABEL { get; set; }
        public string RELEASE_KEY_LABEL { get; set; }
        public string LINE_KEY_LABEL { get; set; }
        public string FUNCTIONALITY_LABEL { get; set; }
        public string RELATED_DEVELOPMENT_LABEL { get; set; }
        public string ISSUE_TYPE_LABEL { get; set; }
        public string FIXED_IN_BUILD_LABEL { get; set; }
        public string BUG_AFFECTEDBUILD_LABEL { get; set; }
        public string BUG_PROJECT_MODULE { get; set; }

        public string BUG_DELIVERABLE_VERSIONS { get; set; }


        public Dictionary<string, string> DEV_STATUS_MAPPING { get; set; }
        public string DEV_STATUS_MAPPING_DEFAULT { get; set; }
        public Dictionary<string, string> DEV_ESTIMATE_TYPE_MAPPING { get; set; }
        public string DEV_ESTIMATE_TYPE_MAPPING_DEFAULT { get; set; }
        public Dictionary<string, string> TASK_TYPE_MAPPING { get; set; }
        public string TASK_TYPE_MAPPING_DEFAULT { get; set; }
        public Dictionary<string, string> UAT_STATUS_MAPPING { get; set; }
        public string UAT_STATUS_MAPPING_DEFAULT { get; set; }
        public Dictionary<string, string> UAT_PRIORITY_MAPPING { get; set; }
        public Dictionary<string, string> UAT_SEVERITY_MAPPING { get; set; }
        public Dictionary<string, string> UAT_CATEGORY_MAPPING { get; set; }
        public string UAT_CATEGORY_MAPPING_DEFAULT { get; set; }
        public Dictionary<string, string> UAT_TYPE_MAPPING { get; set; }
        public Dictionary<string, string> BUG_STATUS_MAPPING { get; set; }
        public Dictionary<string, string> BUG_PRIORITY_MAPPING { get; set; }
        public Dictionary<string, string> BUG_TYPE_MAPPING { get; set; }
        public Dictionary<string, string> BUG_CAUSE_TYPE_MAPPING { get; set; }
        public string BUG_CAUSE_TYPE_DEFAULT { get; set; }
        public Dictionary<string, string> BUG_SUGGESTED_ACTION_TYPE_MAPPING { get; set; }
        public string BUG_SUGGESTED_ACTION_TYPE_DEFAULT { get; set; }

        public Dictionary<string, string> DEV_LINE_MAPPING { get; set; }
    }
}
