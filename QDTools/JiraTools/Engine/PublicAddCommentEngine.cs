using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class PublicAddCommentEngine
    {
        #region Private properties
        private readonly AddCommentEngine engine;
        #endregion

        public PublicAddCommentEngine(AddCommentEngine engine)
        {
            this.engine = engine;
        }

        #region Public methods

        public bool Execute(string issueKey, string author, string body)
        {
            var comment = this.engine.Execute(issueKey, author, body);

            return comment == null ? 
                false:
                true;
        }

        #endregion

        #region Private methods

        #endregion
    }
}
