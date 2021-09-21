using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace JiraIssueProducer.Configuration
{
    public class ConfigurationContainer
    {
        #region Private properties

        private JiraProducerParameters parameters;


        #endregion

        #region Constructor

        public ConfigurationContainer()
        {
            this.parameters = Readconfiguration();
        }


        #endregion


        #region public properties

        public JiraProducerParameters Configuration { get { return parameters; } }

        #endregion

        private JiraProducerParameters Readconfiguration()
        {
            var cfgPath = GetAssemblyDirectory() + "JiraProducer.json";

            var cfgResource = string.IsNullOrWhiteSpace(cfgPath) ?
                null :
                File.ReadAllText(cfgPath);

            if (cfgResource == null)
                throw new Exception("CannotFindEmbeddedResourceNamedXFormat " + cfgPath);

            return JsonConvert.DeserializeObject<JiraProducerParameters>(cfgResource);
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


    }
}
