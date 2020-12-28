using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromGemini.Jira.Model
{

    public class Errors
    {
    }

    public class ErrorCollection
    {
        public List<object> errorMessages { get; set; }
        public Errors errors { get; set; }
    }

    public class Transition
    {
        public int status { get; set; }
        public ErrorCollection errorCollection { get; set; }
    }

    public class AddIssueResponse
    {
        public string id { get; set; }
        public string key { get; set; }
        public string self { get; set; }
        public Transition transition { get; set; }
    }
}
