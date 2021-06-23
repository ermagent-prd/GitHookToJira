using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraTools.Model
{
    public class AddCustomFieldEngine
    {
        #region Public methods

        public void Execute(CreateIssueInfo jiraIssue, string fieldName, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
                return;

            jiraIssue.CustomFields.Add(new CustomFieldInfo(fieldName, fieldValue));
        }

        #endregion

    }
}
