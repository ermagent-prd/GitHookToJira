using System.Collections.Generic;

namespace SvnTools.Engine
{
    public class SvnLookResult
    {
        public SvnLookResult(IEnumerable<string> paths, IEnumerable<string> trackingUssue)
        {
            Paths = paths;
            TrackingIssues = trackingUssue;
        }

        public IEnumerable<string> Paths { get; }

        public IEnumerable<string> TrackingIssues { get; }
    }
}
