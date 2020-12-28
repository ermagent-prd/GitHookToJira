using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTools.Model
{
    public class RequestInfo
    {
        public string User { get; set; }

        public string Token { get; set; }

        public string JiraServerUrl { get; set; }

        public string Api { get; set; }

    }
}
