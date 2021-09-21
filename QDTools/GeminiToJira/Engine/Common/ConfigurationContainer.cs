using System;
using System.Collections.Generic;
using System.Linq;
using GeminiToJira.Parameters.Import;

namespace GeminiToJira.Engine.Common
{
    public class ConfigurationContainer
    {
        #region Private properties

        private GeminiToJiraParameters parameters;


        #endregion

        #region Constructor

        public ConfigurationContainer(ImportCfgType cfgKey)
        {
            this.parameters = Readconfiguration(cfgKey);
        }


        #endregion


        #region public properties

        public GeminiToJiraParameters Configuration { get { return parameters; } }

        #endregion

        private GeminiToJiraParameters Readconfiguration(ImportCfgType cfgKey)
        {
            //var cfgManager = new ConfigurationManager();
            var cfgManager = new FileConfigurationManager();
            var configurationSetup = cfgManager.Execute(cfgKey);
            return configurationSetup;
        }

    }
}
