namespace GeminiIssueProducer
{
    internal interface IGeminiCommandOutput
    {
        int ErrorCode { get; }
        bool Result { get; }
    }

    internal interface IGeminiCommandOutput<T> : IGeminiCommandOutput
    {
        T Value { get; }
    }
}