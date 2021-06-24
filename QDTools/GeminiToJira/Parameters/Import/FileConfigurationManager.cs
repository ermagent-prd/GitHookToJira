using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    internal class FileConfigurationManager 
    {
        #region private property

        private readonly Dictionary<ImportCfgType, string> configurations;

        #endregion

        #region Constructor

        public FileConfigurationManager()
        {

            this.configurations = getConfigurations();
        }


        #endregion

        #region Public methods

        public GeminiToJiraParameters Execute(ImportCfgType importType)
        {
            configurations.TryGetValue(importType, out string cfgPath);

            var cfgResource = string.IsNullOrWhiteSpace(cfgPath) ?
                null :
                File.ReadAllText(cfgPath);

            if (cfgResource == null)
                throw new Exception("CannotFindEmbeddedResourceNamedXFormat " + cfgPath);

            return JsonConvert.DeserializeObject<GeminiToJiraParameters>(cfgResource);
        }
        #endregion


        #region Private methods

        private Dictionary<ImportCfgType, string> getConfigurations()
        {
            var cfgDir = getConfigDirectory();

            var cfgList = new Dictionary<ImportCfgType, string>();

            cfgList.Add(ImportCfgType.Test, string.Concat(cfgDir, "Test.json"));
            cfgList.Add(ImportCfgType.ERM, string.Concat(cfgDir, "ERM.json"));
            cfgList.Add(ImportCfgType.SSSP, string.Concat(cfgDir, "SSSP.json"));
            cfgList.Add(ImportCfgType.RMS5, string.Concat(cfgDir, "RMS5.json"));
            cfgList.Add(ImportCfgType.ERMPAT, string.Concat(cfgDir, "ERMPAT.json"));
            cfgList.Add(ImportCfgType.ILIASBSM, string.Concat(cfgDir, "ILIAS-BSM.json"));

            return cfgList;
        }

        private string getConfigDirectory()
        {
            var assemblyDir = GetAssemblyDirectory();

            return string.Concat(assemblyDir, "Parameters\\Import\\CfgFiles\\");
        }

        private string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return GetFullPathWithEndingSlashes(Path.GetDirectoryName(path));
        }

        private string GetFullPathWithEndingSlashes(string input)
        {
            return Path.GetFullPath(input)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
        }
        #endregion


    }
}
