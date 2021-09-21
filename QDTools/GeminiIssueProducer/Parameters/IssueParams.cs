namespace GeminiIssueProducer.Parameters
{
    public class IssueParams
    {
        public IssueFixedParams FixedParams { get; }
        public IssueFreeTextParams FreeParams { get; }
        public IssueConfigParameters ConfigParams { get; }

        public IssueParams(
            string project,
            string type,
            string issueType,
            string severity,
            string functionality,
            string resources,
            string title,
            string description,
            string build,
            string attachments,
            string comment) : 
                this(
                    new IssueFixedParams(
                        project,
                        type,
                        issueType,
                        severity,
                        functionality),
                    new IssueFreeTextParams(
                        resources,
                        title,
                        description,
                        build,
                        attachments,
                        comment))
        { }

        public IssueParams(
            IssueFixedParams fixedParams,
            IssueFreeTextParams freeParams)
        {
            FixedParams = fixedParams;
            FreeParams = freeParams;

            ConfigParams = new IssueConfigParameters();
        }
    }
}
