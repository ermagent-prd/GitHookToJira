using System.Collections.Generic;

namespace GeminiIssueProducer
{
    public enum GeminiIssueProducerOptions
    {
        None,
        AddOrUpdate,
        UpdateNotClosed
    }

    internal class GeminiIssueProducerOptionsParser
    {
        private static Dictionary<string, GeminiIssueProducerOptions> stringOptionsValues =
            new Dictionary<string, GeminiIssueProducerOptions>()
            {   { GeminiConstants.OPT_ADD_OR_UPDATE, GeminiIssueProducerOptions.AddOrUpdate},
                { GeminiConstants.OPT_UPDATE_NOT_CLOSED, GeminiIssueProducerOptions.UpdateNotClosed } };

        public static GeminiIssueProducerOptions Parse(string value)
        {
            return stringOptionsValues[value];
        }

        public static bool TryParse(string value, out GeminiIssueProducerOptions option)
        {
            if(stringOptionsValues.ContainsKey(value))
            {
                option = Parse(value);
                return true;
            }

            option = GeminiIssueProducerOptions.None;
            return false;
        }
    }
}
