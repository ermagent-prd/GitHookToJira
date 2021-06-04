using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JiraReport.Parameters.Export
{
    internal class ConfigurationManager
    {
        #region private property

        private const string resourceNameSpace = "JiraReport.Parameters.Export.CfgFiles.";

        #region private property

        private readonly Dictionary<ExportReportType, string> configurations;

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

        public ExcelConfiguration Execute(ExportReportType importType)
        {
            Assembly resourceAssembly = Assembly.GetExecutingAssembly();

            configurations.TryGetValue(importType, out string cfgName);

            var cfgResource = string.IsNullOrWhiteSpace(cfgName) ?
                null :
                this.loader.Execute(resourceAssembly, cfgName);

            if (cfgResource == null)
                throw new Exception("CannotFindEmbeddedResourceNamedXFormat " + cfgName);

            return JsonConvert.DeserializeObject<ExcelConfiguration>(cfgResource);
        }
        #endregion

        #region Private methods

        private Dictionary<ExportReportType, string> getConfigurations()
        {
            var cfgList = new Dictionary<ExportReportType, string>();

            cfgList.Add(ExportReportType.Test, string.Concat(resourceNameSpace, "Test.json"));      
            

            return cfgList;
        }
        #endregion
        

    }
}
