using System;
using System.Collections.Generic;
using System.Net;
using SharpSvn;
using SvnTools.Engine;
using SvnTools.Model;
using SvnTools.Parameters;

namespace SvnTools
{
    public class RevisionPropertiesEngine
    {
        private readonly ISvnToolsParameters parameters;

        private readonly TrackingIssueGetter trackingGetter;

        private const string authorKey = "svn:author";
        private const string logKey = "svn:log";
        private const string dateKey = "svn:date";

        internal RevisionPropertiesEngine(
            ISvnToolsParameters parameters,
            TrackingIssueGetter trackingGetter)
        {
            this.parameters = parameters;
            this.trackingGetter = trackingGetter;
        }

        public RevisionProperties Execute(int revision)
        {
            return Execute(this.parameters.ServerUrl, revision);
        }


        public RevisionProperties Execute(string svnRepoPath, int revision)
        {
            using (var client = new SvnClient())
            {
                if (!this.parameters.WindowsAuth)
                {
                    client.Authentication.Clear();
                    client.Authentication.DefaultCredentials = new NetworkCredential(
                        this.parameters.User,
                        this.parameters.Password);
                }

                var repoUri = new Uri(svnRepoPath);

                SvnPropertyCollection properties = null;

                client.GetRevisionPropertyList(repoUri,
                    new SvnRevision(revision),
                    out properties);

                if (properties == null)
                    return null;

                var author = getProperty(properties, authorKey);
                var log = getProperty(properties, logKey);
                var date = getProperty(properties, dateKey);

                var trackingInfo = this.trackingGetter.Execute(
                    this.parameters.TrackingIssuePattern, 
                    log);

                return new RevisionProperties(
                    revision,
                    log,
                    date,
                    author,
                    svnRepoPath,
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
