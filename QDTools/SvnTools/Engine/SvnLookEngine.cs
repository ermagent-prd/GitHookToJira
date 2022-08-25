using SharpSvn;
using SvnTools.Engine;
using SvnTools.Parameters;
using System.Collections.Generic;
using System.Linq;

namespace SvnTools
{
    public class SvnLookEngine
    {
        private const string logKey = "svn:log";

        private readonly ISvnToolsParameters parameters;

        private readonly TrackingIssueGetter trackingGetter;

        public SvnLookEngine(
            TrackingIssueGetter trackingGetter,
            ISvnToolsParameters parameters)
        {
            this.trackingGetter = trackingGetter;
            this.parameters = parameters;
        }

        public SvnLookResult Execute(string repoPath, string transactionName)
        {
            string[] args = new string[2];
            args[0] = repoPath;
            args[1] = transactionName;

            SvnHookArguments ha;
            if (!SvnHookArguments.ParseHookArguments(args, SvnHookType.PreCommit, false, out ha))
            {
                return null;
            }
            using (SvnLookClient cl = new SvnLookClient())
            {
                SvnChangeInfoEventArgs ci;
                cl.GetChangeInfo(ha.LookOrigin, out ci);

                var log = getProperty(ci.RevisionProperties, logKey);

                var trackingInfo = this.trackingGetter.Execute(
                    this.parameters.TrackingIssuePattern,
                    log);

                return
                    new SvnLookResult(
                        ci.ChangedPaths.Select(p => p.Path),
                        trackingInfo);

            }
        }

        private string getProperty(SvnPropertyCollection properties, string key)
        {
            if (!properties.Contains(key))
                return null;

            return properties[key].StringValue;

        }

    }
}
