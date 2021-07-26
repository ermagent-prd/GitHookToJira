using System.Linq;
using Countersoft.Gemini.Commons.Dto;
using JiraTools.Model;
using static GeminiToJira.Engine.JiraAccountIdEngine;

namespace GeminiToJira.Engine
{
    public class AssigneeEngine
    {
        #region Private properties

        private readonly JiraAccountIdEngine accountEngine;


        #endregion

        #region Constructor
        public AssigneeEngine(JiraAccountIdEngine accountEngine)
        {
            this.accountEngine = accountEngine;
        }

        #endregion

        #region Public methods

        public void Execute(IssueDto geminiIssue, CreateIssueInfo jiraIssue, string defaultAccount)
        {
            string assignee = getFromOwner(geminiIssue, defaultAccount);

            if (string.IsNullOrWhiteSpace(assignee))
                assignee = getFromUat(geminiIssue, defaultAccount);

            if (!string.IsNullOrWhiteSpace(assignee))
                jiraIssue.Assignee = assignee;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// recupera l'assegnatario per le UAT in base a unica risorsa o prima risorsa che cambia lo stato da assigned a in progress
        /// </summary>
        /// <param name="geminiIssue"></param>
        /// <param name="defaultAccount"></param>
        /// <returns></returns>
        private string getFromUat(IssueDto geminiIssue, string defaultAccount)
        {
            if (geminiIssue.Project.Name != "ESF - UAT")
                return null;

            var accountName = getUATAccountName(geminiIssue);

            if (string.IsNullOrWhiteSpace(accountName))
                return null;

            var account = this.accountEngine.Execute(accountName, defaultAccount);

            if (account != null && !string.IsNullOrWhiteSpace(account.AccountId))
                return account.AccountId;

            return null;

        }

        private string getUATAccountName(IssueDto geminiIssue)
        {
            if (geminiIssue.Resources.Count == 1)
                return geminiIssue.ResourceNames;

            //Prendo la prima risorsa che ha modificato lo status da assigned a altro
            var historyItem = geminiIssue.History.OrderBy(i => i.Date).FirstOrDefault(
                h => h.Entity.FieldChanged == "Status" &&
                h.Entity.ValueBefore == "Assigned");

            if (historyItem != null)
                return historyItem.Author;

            return null;
        }

        private string getFromOwner(IssueDto geminiIssue, string defaultAccount)
        {
            var owner = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Owner");

            if (owner == null || string.IsNullOrWhiteSpace(owner.FormattedData))
                return null;

            var account = this.accountEngine.Execute(owner.FormattedData, defaultAccount);

            if (account != null && !string.IsNullOrWhiteSpace(account.AccountId))
                return account.AccountId;

            return null;
        }


        #endregion
    }
}
