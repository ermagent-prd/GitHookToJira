using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraTools.Model
{
    public class RemoteLinkInfo
    {
        public RemoteLinkInfo(string title, string url, string sumnary)
        {
            Title = title;
            Url = url;
            Sumnary = sumnary;
        }

        public string Title { get; }

        public string Url { get; }

        public string Sumnary {get;}



    }
}
