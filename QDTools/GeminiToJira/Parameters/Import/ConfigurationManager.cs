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

        private readonly Dictionary<ImportCfgType, string> configurations;

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

        public GeminiToJiraParameters Execute(ImportCfgType importType)
        {
            Assembly resourceAssembly = Assembly.GetExecutingAssembly();

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

        private Dictionary<ImportCfgType, string> getConfigurations()
        {
            var cfgList = new Dictionary<ImportCfgType, string>();

            cfgList.Add(ImportCfgType.Test, string.Concat(resourceNameSpace, "Test.json"));
            cfgList.Add(ImportCfgType.ERM, string.Concat(resourceNameSpace, "ERM.json"));
            cfgList.Add(ImportCfgType.SSSP, string.Concat(resourceNameSpace, "SSSP.json"));
            cfgList.Add(ImportCfgType.RMS5, string.Concat(resourceNameSpace, "RMS5.json"));
            cfgList.Add(ImportCfgType.ERMPAT, string.Concat(resourceNameSpace, "ERMPAT.json"));
            cfgList.Add(ImportCfgType.ILIASBSM, string.Concat(resourceNameSpace, "ILIAS-BSM.json"));

            return cfgList;
        }
        #endregion


    }
}
