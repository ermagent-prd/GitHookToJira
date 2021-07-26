using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiToJira.Engine.Common
{
    public class URLChecker
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        public string Execute(string originalUrl)
        {
            return
                originalUrl.Replace("<p>", "").Replace("</p>", "");
        }

        #endregion

        #region Private methods


        #endregion
    }
}
