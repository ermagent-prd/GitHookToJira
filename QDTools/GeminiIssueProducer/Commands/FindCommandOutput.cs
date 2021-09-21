namespace GeminiIssueProducer.Commands
{
    internal class FindCommandOutput : SimpleCommandOutput, IGeminiCommandOutput<int[]>
    {
        public int[] Value { get; }

        public FindCommandOutput(bool result, int errorCode, int[] value) : base(result, errorCode)
        {
            Value = value;
        }
    }
}
