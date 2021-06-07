using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using SvnToJira.Parameters;

namespace SvnToJira.Engine
{
    internal class ConfigurationLoader
    {

        #region Public methods

        public SvnToJiraParameters Execute()
        {
            Assembly resourceAssembly = Assembly.GetExecutingAssembly();

            string cfgFile = Path.GetDirectoryName(resourceAssembly.Location) + "\\SvnToJira.json";

            string jsonString = File.ReadAllText(cfgFile);

            return JsonConvert.DeserializeObject<SvnToJiraParameters>(jsonString);
        }

        #endregion

    }
}
