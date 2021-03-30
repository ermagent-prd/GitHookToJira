namespace JiraToJira.Container.Jira
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
                
    }
}
