using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GeminiToJira.Engine
{
    public class ParseCommentEngine
    {
        private readonly string HTML_TAG_PATTERN = "<.*?>";
        
        public string Execute(string comment)
        {
            return Regex.Replace(comment, HTML_TAG_PATTERN, string.Empty);
        }
    }
}
