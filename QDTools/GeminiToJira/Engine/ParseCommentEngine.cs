using System.Text.RegularExpressions;

namespace GeminiToJira.Engine
{
    public static class ParseCommentEngine
    {
        private readonly static string HTML_TAG_PATTERN = "<.*?>";

        public static string Execute(string comment)
        {
            return Regex.Replace(comment, HTML_TAG_PATTERN, string.Empty);
        }
    }
}
