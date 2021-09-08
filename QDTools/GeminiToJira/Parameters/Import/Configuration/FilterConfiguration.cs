using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace GeminiToJira.Parameters.Import
{
    public class FilterConfiguration
    {
        public List<string> SELECTED_ISSUES { get; set; }

        public List<string> SKIPPED_ISSUES { get; set; }

        public List<string> INVALID_STATUSES { get; set; }

        public string STORY_PROJECT_ID { get; set; }
        public string STORY_TYPES { get; set; }
        public List<string> STORY_RELEASES { get; set; }
        public List<string> STORY_LINES { get; set; }
        public string STORY_RELEASE_KEY { get; set; }
        public string STORY_LINE_KEY { get; set; }
        public bool STORY_INCLUDED_CLOSED { get; set; }
        public string TASK_PROJECT_ID { get; set; }
        public string TASK_TYPES { get; set; }
        public List<string> TASK_RELEASES { get; set; }
        public List<string> TASK_LINES { get; set; }
        public string TASK_RELEASE_KEY { get; set; }
        public string TASK_LINE_KEY { get; set; }
        public bool TASK_INCLUDED_CLOSED { get; set; }
        public string UAT_PROJECT_ID { get; set; }
        public string UAT_CREATED_FROM { get; set; }

        public int UAT_DAYS_BLOCK { get; set; }

        public bool UAT_GROUP_DEPENDENCIES { get; set; }
        public bool UAT_INCLUDED_CLOSED { get; set; }
        public List<string> UAT_FUNCTIONALITY { get; set; }

        public List<string> UAT_RELATED_DEV { get; set; }

        public HashSet<string> UAT_SELECTED_ITEMS { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UAT_CHECK_RELATED_DEV { get; set; }

        public string ERMBUG_PROJECT_ID { get; set; }
        public bool ERMBUG_INCLUDED_CLOSED { get; set; }

        public string BUG_PRODUCT { get; set; }

        public List<string> BUG_RELEASES { get; set; }

        public HashSet<string> BUG_STATUS { get; set; }

        public HashSet<string> BUG_SKIPPED_STATUS { get; set; }

        public HashSet<string> BUG_SELECTED_ITEMS { get; set; }

        public string BUG_CREATED_DATE_FROM { get; set; }

        public string BUG_CREATED_DATE_TO { get; set; }
    }
}
