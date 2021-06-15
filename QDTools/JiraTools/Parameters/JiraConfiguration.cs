

namespace JiraTools.Parameters
{
    public class JiraConfiguration
    {
        public string User { get; set; }

        public string Token { get; set; }

        public string Url { get; set; }

        public string IssueApi { get; set; }

        public int MaxIssuesPerRequest { get; set; }

        public bool ImportStory { get; set; }
        public bool ImportTask { get; set; }
        public bool ImportUat { get; set; }
        public bool ImportBug { get; set; }
        public string EpicTypeCode { get; set; }
        public string StoryTypeCode { get; set; }
        public string StorySubTaskTypeCode { get; set; }
        public string TaskTypeCode { get; set; }
        public string SubTaskTypeCode { get; set; }
        public string UatTypeCode { get; set; }
        public string BugTypeCode { get; set; }

        public string DefaultAccount { get; set; }

    }
}
