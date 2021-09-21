using System;
using System.Linq;

namespace GeminiIssueProducer.Parameters
{
    public class IssueFreeTextParams
    {
        #region Properties

        public string[] ResourceNames { get; }

        public string Title { get; }

        public string Description { get; }

        public string AffectedBuild { get; }

        public string[] Attachments { get; }

        public string Comment { get; }

        #endregion

        #region Constructor

        public IssueFreeTextParams(
            string[] resourceNames,
            string title,
            string description,
            string affectedBuild,
            string[] attachments,
            string comment)
        {
            ResourceNames = resourceNames;
            Title = title;
            Description = description;
            AffectedBuild = affectedBuild;
            Attachments = attachments;
            Comment = comment;
        }

        public IssueFreeTextParams(
        string resourceNames,
        string title,
        string description,
        string affectedBuild,
        string attachments,
        string comment)
        {
            ResourceNames = ParseCSV(resourceNames);
            Title = title;
            Description = description;
            AffectedBuild = affectedBuild;
            Attachments = ParseCSV(attachments);
            Comment = comment;
        }


        #endregion

        #region Private methods

        private string[] ParseCSV(string value)
        {
            return
                value
                .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();
        }

        #endregion
    }
}
