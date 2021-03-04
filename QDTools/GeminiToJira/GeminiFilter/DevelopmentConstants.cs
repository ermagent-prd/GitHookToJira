using System.Collections.Generic;

namespace GeminiToJira.GeminiFilter
{
    public static class DevelopmentConstants
    {
        public static readonly string DEVELOPMENT_PROJECT_ID = "36";  //developmente project
        public static readonly bool DEVELOPMENT_INCLUDED_CLOSED = true;
        public static readonly string DEVELOPMENT_TYPES = "|Developer|Task|";
        public static readonly List<string> DEVELOPMENT_RELEASES = new List<string>() {
                "ERMAS",
                "ERMAS 5.24.0",
                "ERMAS 5.24.1",
                "ERMAS 5.25.0",
                "ERMAS 5.26.0",
                "ERMAS 5.27.0",
                "ERMAS 5.28.0",
                "ERMAS 5.29.0",
                "0.0.0.0"
            };

        public static readonly List<string> DEVELOPMENT_LINES = new List<string> {
                "BSM",
                "ILIAS",
                "ILIAS-STA",
                "Other"
            };

        public static readonly string DEVELOPMENT_RELEASE_KEY = "Release Version";
        public static readonly string DEVELOPMENT_LINE_KEY = "DVL";

    }
}
