using Countersoft.Gemini.Commons.Dto;

namespace GeminiIssueProducer.Commands
{
    internal class CreateCommandOutput : SimpleCommandOutput, IGeminiCommandOutput<IssueDto>
    {
        public IssueDto Value { get; }

        public CreateCommandOutput(bool result, int errorCode, IssueDto value) : base(result, errorCode)
        {
            Value = value;
        }
    }
}
