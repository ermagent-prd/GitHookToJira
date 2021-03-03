using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    public sealed class EmbeddedCfgLoader
    {
        #region public methods

        public String Execute(Assembly sourceAssembly, String embeddedResourceName)
        {
            StringBuilder sb = new StringBuilder();

            // Check input
            if (String.IsNullOrWhiteSpace(embeddedResourceName))
                throw new ArgumentNullException(nameof(embeddedResourceName) + " InvalidNullInputFormat");

            if (sourceAssembly == null)
                throw new Exception("InvalidNullInputFormat " + embeddedResourceName); 

            var stream = sourceAssembly.GetManifestResourceStream(embeddedResourceName);

            if (stream == null)
                throw new Exception("CannotFindEmbeddedResourceNamedXFormat " + embeddedResourceName);

            using (StreamReader rdr = new StreamReader(stream))
                sb.AppendLine(rdr.ReadToEnd());

            return sb.ToString();
        }

        #endregion
    }
}
