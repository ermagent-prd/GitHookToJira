using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJira.Engine
{
    public  class ActionResult
    {
        public ActionResult(bool ok, string message)
        {
            Ok = ok;
            Message = message;
        }

        public static ActionResult Passed()
        { 
            return new ActionResult(true, string.Empty);
        }

        public bool Ok { get; }

        public string Message { get; }
    }
}
