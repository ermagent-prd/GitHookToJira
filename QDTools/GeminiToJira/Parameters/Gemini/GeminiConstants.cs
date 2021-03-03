namespace GeminiToJira.Parameters
{
    /// <summary>
    /// Costanti temporanee. Queste informazioni devono essere recuperate da file di configurazione
    /// </summary>
    internal static class GeminiConstants
    {
        /// <summary>
        /// ERM Gemini Url
        /// </summary>
        public const string GeminiUrl = "https://erm-swfactory.prometeia.com/Gemini";

        public const string ErmPrefix = "ERM-";
        public const string UatPrefix = "UAT-";
        public const string ErmBugPrefix = "ERMBUG-";

        public const string SAVING_PATH = @"C:\GeminiPorting\AttachmentDownloaded\";
        public const string GEMINI_PATH = "https://erm-swfactory.prometeia.com/Gemini/project/";

        public const string GroupType = "Group";
    }
}
