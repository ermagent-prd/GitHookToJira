namespace GeminiIssueProducer
{
    internal class SimpleCommandOutput : IGeminiCommandOutput
    {
        public bool Result { get; }
        public int ErrorCode { get; }


        public SimpleCommandOutput(bool result, int errorCode)
        {
            Result = result;
            ErrorCode = errorCode;
        }
    }
}
