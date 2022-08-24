using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJira.Engine
{
    internal class TrackingIssueToCheckFields
    {
        public TrackingIssueToCheckFields(string trackingIssue, string issueTypeId)
        {
            TrackingIssue = trackingIssue;
            IssueTypeId = issueTypeId;
        }

        public string TrackingIssue { get; }

        public string IssueTypeId { get; }


    }
}
