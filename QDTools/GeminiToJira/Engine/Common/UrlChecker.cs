using System;

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

        public bool IsValid(string urlAddress)
        {
            Uri uriResult;
            return Uri.TryCreate(urlAddress, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        #endregion

        #region Private methods


        #endregion
    }
}
