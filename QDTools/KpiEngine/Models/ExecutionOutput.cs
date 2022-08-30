using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal enum ExecutionResult
    {
        Ok,
        Warning,
        Error
    }

    internal class ProcessResult
    {
        public ExecutionResult Result { get; }
        public string Message { get; }
    }

}
