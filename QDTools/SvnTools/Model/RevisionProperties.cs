using System;
using System.Collections.Generic;
using System.Linq;

namespace SvnTools.Model
{
    public class RevisionProperties
    {
        #region public properties

        public int Revision { get; }

        public string Log { get; }

        public String Date { get; }

        public String Author { get; }

        public IEnumerable<String> TrackingIssues { get; }

        #endregion

        #region Constructor

        public RevisionProperties(int revision, string log, string date, string author, IEnumerable<String> trackingIssues)
        {
            this.Revision = revision;
            Log = log;
            Date = date;
            Author = author;
            this.TrackingIssues = trackingIssues;
        }

        #endregion

    }
}
