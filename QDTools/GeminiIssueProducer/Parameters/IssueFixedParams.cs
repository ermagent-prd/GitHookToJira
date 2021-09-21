using System.Collections.Generic;

namespace GeminiIssueProducer.Parameters
{
    public class IssueFixedParams
    {
        #region Private fields

        private Dictionary<string, int> constantIdMap = 
            new Dictionary<string, int>()
            {
                { GeminiConstants.PROJ_ESF_UAT, GeminiConstants.PROJ_ESF_UAT_ID },
                { GeminiConstants.TYPE_INTERNAL_UAT, GeminiConstants.TYPE_INTERNAL_UAT_ID },
                { GeminiConstants.SEVERITY_MEDIUM, GeminiConstants.SEVERITY_MEDIUM_ID },
            };

        private int userId;

        #endregion

        #region Properties

        public int ProjectIdValue { get; }
        public int TypeIdValue { get; }
        public string IssueType { get; }
        public int SeverityIdValue { get; }
        public string Functionality { get; }
        public int UserId { get { return userId; } }
        
        #endregion

        #region Constructor

        public IssueFixedParams(
            string project,
            string type,
            string issueType,
            string severity,
            string functionality)
        {
            ProjectIdValue = constantIdMap[project];

            TypeIdValue = constantIdMap[type];

            IssueType = issueType;

            SeverityIdValue = constantIdMap[severity];

            Functionality = functionality;
        }

        #endregion

        #region Public methods
        
        public void SetUserId(int userId)
        {
            this.userId = userId;
        }

        #endregion

    }
}
