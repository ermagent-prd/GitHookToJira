namespace JiraToJira.Container.Jira
{
    /// <summary>
    /// Costanti temporanee. Queste informazioni devono essere recuperate da file di configurazione
    /// </summary>
    internal static class JiraConstants
    {
        public const string User = "loredana.ribatto@prometeia.com"; //"paolo.luca@prometeia.com";

        public const string Token = "4T8gzmM8rSsYPjuIRECZC4D2"; //"1l9yaa74u6wqg4lbhitOE7C7";

        public const string JiraUrl = "https://prometeia-wam.atlassian.net/";

        public const string IssueApi = "rest/api/3/issue/";

        public const int MaxIssuesPerRequest = 10; //Max 100 ??
                
    }
}
