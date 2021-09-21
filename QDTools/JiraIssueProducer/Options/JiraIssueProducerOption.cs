using System.Collections.Generic;

namespace JiraIssueProducer
{

    public enum JiraIssueProducerOption
    {
        None,
        AddOrUpdate,
        UpdateNotClosed
    }

    internal class JiraIssueProducerOptionsParser
    {
        private static Dictionary<string, JiraIssueProducerOption> stringOptionsValues =
            new Dictionary<string, JiraIssueProducerOption>()
            {   { CommandOptions.OPT_ADD_OR_UPDATE, JiraIssueProducerOption.AddOrUpdate},
                { CommandOptions.OPT_UPDATE_NOT_CLOSED, JiraIssueProducerOption.UpdateNotClosed } };

        public JiraIssueProducerOption Parse(string value)
        {
            return stringOptionsValues[value];
        }

        public bool TryParse(string value, out JiraIssueProducerOption option)
        {
            if(stringOptionsValues.ContainsKey(value))
            {
                option = Parse(value);
                return true;
            }

            option = JiraIssueProducerOption.None;
            return false;
        }
    }
}
