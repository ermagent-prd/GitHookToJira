namespace GeminiIssueProducer.Parameters
{
    public class IssueConfigParameters
    {
        public int AffectedBuildId { get { return GeminiConstants.CF_AFFECTED_BUILD_ID; } }
        public int FunctionalityId { get { return GeminiConstants.CF_FUNCTIONALITY_ID; } }
        public int IssueTypeId { get { return GeminiConstants.CF_ISSUE_TYPE_ID; } }
    }
}
