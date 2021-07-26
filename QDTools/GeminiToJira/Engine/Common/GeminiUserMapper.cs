using System;
using System.Collections.Generic;

namespace GeminiToJira.Engine.Common
{
    public class GeminiUserMapper
    {
        #region Private properties

        private readonly Lazy<Dictionary<string, string>> mapping = null;


        #endregion

        #region Constructor
        public GeminiUserMapper(ConfigurationContainer configurationManager)
        {
            this.mapping = new Lazy<Dictionary<string, string>>(() => { return configurationManager?.Configuration?.Mapping?.USER_MAPPING; });
        }

        #endregion

        #region Public methods

        public string Execute(string userName)
        {
            if (mapping?.Value == null)
                return userName;

            if (this.mapping.Value.TryGetValue(userName, out string mappedUser))
                return mappedUser;

            return userName;
        }


        #endregion
    }
}
