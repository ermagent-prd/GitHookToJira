using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class EditCustomFieldEngine
    {

        private readonly ServiceManagerContainer requestFactory;

        public EditCustomFieldEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        public Issue Execute(string issueKey, string fieldName, string fieldValue)
        {
            var task = editField(issueKey,fieldName, fieldValue);

            if (task == null)
                return null;

            task.Wait();

            return task.Result;
        }

        private async Task<Issue> editField(string issueKey, string fieldName, string fieldValue)
        {
            var jira = this.requestFactory.Service;

            var issue = await jira.Issues.GetIssueAsync(issueKey);

            if (issue == null)
                return null;

            if (string.IsNullOrEmpty(fieldName))
                throw new System.ArgumentNullException(nameof(fieldName));

            if(issue[fieldName] == null)
                return null;

            issue[fieldName] = fieldValue;

            await issue.SaveChangesAsync();

            return issue;
        }


    }
}
