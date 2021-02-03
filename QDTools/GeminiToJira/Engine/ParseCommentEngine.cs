using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GeminiToJira.Engine
{
    public class ParseCommentEngine
    {
        private readonly string HTML_TAG_PATTERN = "<.*?>";
        
        public string Execute(string comment)
        {
            var text = Regex.Replace(comment, HTML_TAG_PATTERN, string.Empty);

            return WebUtility.HtmlDecode(text);

        }
    }
}
