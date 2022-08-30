using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace QDToolsUtilities
{
    public class ConfigurationLoader
    {
        /// <summary>
        /// Load Json configuration from file
        /// </summary>
        /// <typeparam name="T">Parameters class</typeparam>
        /// <param name="cfgPath">Json configuration file path</param>
        /// <param name="defaultCfgFileName">Default configuration file name></param>
        /// <returns>Parameters class instance</returns>
        /// <exception cref="Exception">Thrown in case of not existing configuration file</exception>
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
