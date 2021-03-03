using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    internal class ConfigurationManager
    {
        #region private property

        private const string resourceNameSpace = "GeminiToJira.Parameters.Import.CfgFiles.";

        #region private property

        private readonly Dictionary<ImportType, string> configurations;

        private readonly EmbeddedCfgLoader loader;

        #endregion


        #endregion

        #region Constructor

        public ConfigurationManager()
        {
            this.loader = new EmbeddedCfgLoader();

            this.configurations = getConfigurations();
        }


        #endregion

        #region Public methods

        public GeminiToJiraParameters Execute(ImportType importType, Assembly resourceAssembly)
        {
            configurations.TryGetValue(importType, out string cfgName);

            var cfgResource = string.IsNullOrWhiteSpace(cfgName) ?
                null :
                this.loader.Execute(resourceAssembly, cfgName);

            if (cfgResource == null)
                throw new Exception("CannotFindEmbeddedResourceNamedXFormat " + cfgName);

            return JsonConvert.DeserializeObject<GeminiToJiraParameters>(cfgResource);
        }
        #endregion


        #region Private methods

        private Dictionary<ImportType, string> getConfigurations()
        {
            var cfgList = new Dictionary<ImportType, string>();

            //cfgList.Add(ImportType.Test, string.Concat(resourceNameSpace, "Test.json"));

            cfgList.Add(ImportType.EIB, string.Concat(resourceNameSpace, "ERM.json"));            

            return cfgList;
        }
        #endregion
        

    }
}
