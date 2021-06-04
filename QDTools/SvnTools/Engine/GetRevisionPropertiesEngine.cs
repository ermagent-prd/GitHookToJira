using System;
using System.Collections.Generic;
using SharpSvn;
using SvnTools.Parameters;

namespace SvnTools
{
    public class GetRevisionPropertiesEngine
    {
        private readonly ISvnToolsParameters parameters;

        public GetRevisionPropertiesEngine(ISvnToolsParameters parameters)
        {
            this.parameters = parameters;
        }

        public Dictionary<string,string> Execute(int revision)
        {
            using (var client = new SvnClient())
            {
                var repoUri = new Uri(this.parameters.ServerUrl);

                SvnPropertyCollection properties = null;

                client.GetRevisionPropertyList(repoUri,
                    new SvnRevision(revision),
                    out properties);

                if (properties == null)
                    return null;

                var dict = new Dictionary<string, string>();

                foreach(var k in properties)
                {
                    dict.Add(k.Key, k.StringValue);

                }

                return dict;
            }
        }
    }
}
