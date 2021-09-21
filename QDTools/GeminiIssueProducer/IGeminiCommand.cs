using GeminiIssueProducer.Parameters;

namespace GeminiIssueProducer
{
    internal interface IGeminiCommand
    {
        IGeminiCommandOutput Execute(IssueParams parameters);
    }
}
