using Atlassian.Jira;
using JiraReport.Parameters.Export.Getters;
using JiraReport.Parameters.Export.Getters.Columns;
using System;
using System.Collections.Generic;

namespace JiraReport.Engine
{
    public class ExportFieldsEngine
    {
        private readonly Lazy<Dictionary<string, IExcelFieldGetter>> fieldGetters;
        private readonly RelatedDevEngine relatedDevEngine;

        public ExportFieldsEngine(RelatedDevEngine relatedDevEngine)
        {
            this.relatedDevEngine = relatedDevEngine;
            fieldGetters = getColumnsGetters();
        }

        public string Execute(Issue issue, string fieldName)
        {
            fieldGetters.Value.TryGetValue(fieldName, out IExcelFieldGetter getter);

            if (getter != null)
                return getter.Execute(issue);
            else
                return "";
        }

        private Lazy<Dictionary<string, IExcelFieldGetter>> getColumnsGetters()
        {
            return new Lazy<Dictionary<string, IExcelFieldGetter>>
                (
                () =>
                {
                    var getters = new Dictionary<string, IExcelFieldGetter>();

                    getters.Add("IssueCode", new IssueKeyCodeGetter());
                    getters.Add("Summary", new IssueSummaryGetter());
                    getters.Add("GeminiCode", new IssueGeminiCodeGetter());
                    getters.Add("Status", new IssueStatusGetter());
                    getters.Add("OriginalEstimate", new IssueOriginalEstimateGetter());
                    getters.Add("DueDate", new IssueDueDateGetter());
                    getters.Add("JDECode", new IssueJdeCodeGetter(relatedDevEngine));
                    getters.Add("IssueParentCode", new IssueRelatedDevKeyCodeGetter(relatedDevEngine));

                    return getters;
                }
            );
        }
    }
}
