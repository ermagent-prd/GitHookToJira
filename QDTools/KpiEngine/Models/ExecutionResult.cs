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
        public ProcessResult(ExecutionResult result, string message)
        {
            Result = result;
            Message = message;
        }

        public ExecutionResult Result { get; }
        public string Message { get; }
    }

}
