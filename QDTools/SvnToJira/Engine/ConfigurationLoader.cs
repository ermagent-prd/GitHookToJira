using Newtonsoft.Json;
using SvnToJira.Parameters;
using System;
using System.IO;
using System.Reflection;

namespace SvnToJira.Engine
{
    internal class ConfigurationLoader
    {

        #region Public methods

        public SvnToJiraParameters Execute(string cfgPath)
        {
            string cfgFile = cfgPath;

            if (string.IsNullOrWhiteSpace(cfgPath))
            {
                Assembly resourceAssembly = Assembly.GetExecutingAssembly();
                cfgFile = Path.GetDirectoryName(resourceAssembly.Location) + "\\SvnToJira.json";
            }
            else
            if (!File.Exists(cfgPath))
                throw new Exception(string.Format("Configuration file not found ({0})", cfgPath));

            string jsonString = File.ReadAllText(cfgFile);

            return JsonConvert.DeserializeObject<SvnToJiraParameters>(jsonString);
        }

        #endregion

    }
}
