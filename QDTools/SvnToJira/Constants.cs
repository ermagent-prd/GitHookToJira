using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJira
{

    internal static class EngineOptionArgument
    {
        /// <summary>
        /// Add comment about svn revision to tracking issue
        /// </summary>
        public const string AddJiraComment = "0";
        /// <summary>
        /// Check svn commit related tracking issue
        /// </summary>
        public const string CheckJiraBugFix = "1";

    }

    internal static class EngineOptionValue
    {
        /// <summary>
        /// Add comment about svn revision to tracking issue
        /// </summary>
        public const int AddJiraComment = 0;
        /// <summary>
        /// Check svn commit related tracking issue
        /// </summary>
        public const int CheckJiraBugFix = 1;

    }


}
