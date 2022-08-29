using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace QDToolsUtilities
{
    public class ConfigurationLoader
    {
        public T Execute<T>(string cfgPath, string defaultCfgFileName) where T : class
        {
            string cfgFile = cfgPath;

            if (string.IsNullOrWhiteSpace(cfgPath))
            {
                Assembly resourceAssembly = Assembly.GetExecutingAssembly();
                cfgFile = Path.GetDirectoryName(resourceAssembly.Location) + "\\" + defaultCfgFileName;
            }
            else
            if (!File.Exists(cfgPath))
                throw new Exception(string.Format("Configuration file not found ({0})", cfgPath));

            string jsonString = File.ReadAllText(cfgFile);

            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
