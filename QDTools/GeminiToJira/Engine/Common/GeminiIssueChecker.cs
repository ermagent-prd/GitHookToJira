using System;
using System.Collections.Generic;
using System.Linq;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Parameters.Import;

namespace GeminiToJira.Engine
{
    public class GeminiIssueChecker
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        public bool Execute(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Filter.STORY_RELEASE_KEY);

            if (release == null || string.IsNullOrWhiteSpace(release.FormattedData) || !configurationSetup.Filter.STORY_RELEASES.Contains(release.FormattedData))
                return false;

            var devLine = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == configurationSetup.Filter.STORY_LINE_KEY);

            if (devLine == null && string.IsNullOrWhiteSpace(devLine.FormattedData) || !configurationSetup.Filter.STORY_LINES.Contains(devLine.FormattedData))
                return false;

            return true;
        }

        #endregion

        #region Private methods


        #endregion
    }
}
