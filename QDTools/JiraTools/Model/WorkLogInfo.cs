using System;

namespace JiraTools.Model
{
    public class WorkLogInfo
    {
        public WorkLogInfo(string author, DateTime startDate, string timeSpent, string comment)
        {
            Author = author;
            StartDate = startDate;
            TimeSpent = timeSpent;
            Comment = comment;
        }

        public string Author { get; set; }

        public DateTime StartDate { get; set; }

        public string TimeSpent { get; set; }

        public string Comment { get; set; }

    }
}
