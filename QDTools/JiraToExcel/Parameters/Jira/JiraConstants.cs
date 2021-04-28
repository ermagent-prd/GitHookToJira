namespace JiraToExcel.Parameters
{
    /// <summary>
    /// Costanti temporanee. Queste informazioni devono essere recuperate da file di configurazione
    /// </summary>
    internal static class JiraConstants
    {
        public const string User = "pierluigi.nanni@prometeia.com"; //"paolo.luca@prometeia.com";

        public const string Token = "GOojveJqyGUAgi6mzSAu3FAA"; //"1l9yaa74u6wqg4lbhitOE7C7";

        public const string JiraUrl = "https://prometeia-erm.atlassian.net/";

        public const string IssueApi = "rest/api/3/issue/";

        public const int MaxIssuesPerRequest = 10; //Max 100 ??

        public const string AttachmentPath = @"C:\GeminiPorting\AttachmentDownloaded\";

        public const string LogDirectory = @"C:\GeminiPorting\Log\";

        //10014:UAT,10004:Bug,10000:Epic,10001:Story,10002:Task,10003:Sub-task]

        public const string Epic = "10000";
        public const string StoryType = "10001";
        public const string Task = "10002";
        public const string SubTaskType = "10003";
        public const string UatType = "10014";
        public const string BugType = "10004";
        
    }
}
