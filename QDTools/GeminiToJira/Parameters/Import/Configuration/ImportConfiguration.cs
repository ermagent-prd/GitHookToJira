using AlfrescoTools.Parameters;
using GeminiTools.Parameters;
using JiraTools.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.Parameters.Import
{
    public class GeminiToJiraParameters
    {
        public string JiraProjectCode { get; set; }
        //for developmentImportEngine
        public List<String> ComponentsForDevelopment { get; set; }

        public FilterConfiguration Filter { get; set; }

        public AlfrescoConfiguration Alfresco { get; set; }
        public GeminiConfiguration Gemini { get; set; }
        public JiraConfiguration Jira { get; set; }

        //local path
        public string LogDirectory { get; set; }
        public string AttachmentDownloadedPath { get; set; }


        public IGeminiToolsParameters GeminiParameters { get; set; }
        public IJiraToolsParameters JiraParameters { get; set; }
        public IAlfrescoToolsParameters AlfrescoParameter { get; set; }
}
}
